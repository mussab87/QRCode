using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Utility.Toolkit.Configuration;

namespace Utility.Core.Logging
{
    /// <summary>
    /// Provides a set of methods and properties that help you trace the 
    /// execution of your code. This class cannot be inherited.
    /// </summary>
    [ReflectionPermission(SecurityAction.Assert)]
    public sealed class FileTrace
    {
        public class TraceConfig
        {
            public char functionDelimiter = '=';
            public int functionDelimiterCount = 40;
            public char exceptionDelimiter = 'X';
            public int exceptionDelimiterCount = 40;
            public string formatString = "{{{0}}} {1} (in {2}::{3}.{4} at {5})";

            public ArrayList excludedClasses = new ArrayList(0);
        }
        private static string EventLogName = "My Clinic";

        static readonly TraceConfig config;

        static FileTrace()
        {
            if (!File.Exists("traceConfig.xml"))
            {
                config = new TraceConfig();
                return;
            }
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TraceConfig));
                using (FileStream fs = new FileStream("traceConfig.xml", FileMode.Open))
                using (TextReader tr = new StreamReader(fs, Encoding.UTF8))
                {
                    config = (TraceConfig)serializer.Deserialize(tr);
                }
                var eventLogName = CoreConfiguration.Instance.EventLogSourceName;
                if (!string.IsNullOrWhiteSpace(eventLogName))
                    EventLogName = eventLogName;
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception ex)
            {
                InternalWrite("Caught {0} ({1}) deserialising TraceConfig", ex.GetType().Name, ex.Message);
                config = new TraceConfig();
            }
        }

        static void InternalWrite(string message, params object[] formatObjects)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    System.Diagnostics.Trace.WriteLine(String.Format(message, formatObjects));
                    FileLogger.Instance.LogMessage(String.Format(message, formatObjects));
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e, EventLogName);
            }
        }

        static MethodBase GetCallingMethod()
        {
            MethodBase mb = null;
            try
            {
                StackTrace st = new StackTrace();
                if (st != null)
                {
                    for (int i = 0; i < st.FrameCount; ++i)
                    {
                        mb = st.GetFrame(i).GetMethod();
                        if (mb != null && mb.DeclaringType != null)
                        {
                            // Found a declared method.
                            if (mb.DeclaringType.FullName != typeof(FileTrace).FullName)
                            {
                                break;
                            }
                        }
                        else
                        {
                            // Could not find a declared method.
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e, EventLogName);
            }
            return mb;
        }

        static void WriteLine(string namespaceName, string typeName, string memberName, string message)
        {
            try
            {
                InternalWrite(config.formatString, Thread.CurrentThread.ManagedThreadId.ToString(), message, namespaceName, typeName, memberName, DateTime.Now.ToLongTimeString());
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e, EventLogName);
            }
        }

        /// <summary>
        /// Writes information about the trace to the trace listeners in the Listeners collection.
        /// </summary>
        /// <param name="className">The class from which the trace information is being 
        /// written.</param>
        /// <param name="methodName">The method from which the trace information is being 
        /// written.</param>
        /// <param name="message">A message to write.</param>
        [Obsolete("Use of Trace.WriteLine(className, methodName, message) is deprecated. Please use WriteLine(message) instead", false), Conditional("TRACE")]

        public static void WriteLine(string className, string methodName, string message)
        {
            try
            {
                MethodBase mb = GetCallingMethod();
                if (mb == null || config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                InternalWrite("{" + Thread.CurrentThread.ManagedThreadId.ToString() + "} " + message + " (" + className + "::" + methodName + ") @ " + DateTime.Now.ToUniversalTime());
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e, EventLogName);
            }
        }

        /// <summary>
        /// Writes information about the trace to the trace listeners in the Listeners collection.
        /// </summary>
        /// <param name="message">A message to write.</param>
        [Conditional("TRACE")]
        public static void WriteLine(string message)
        {
            try
            {
                MethodBase mb = GetCallingMethod();
                if (mb == null || config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                WriteLine(mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name, message);
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e, EventLogName);
            }
        }

        /// <summary>
        /// Writes information to the trace listeners
        /// </summary>
        /// <param name="message">A <see cref="String"/> containing zero or more format items.</param>
        /// <param name="args">An <see cref="Object"/> array containing zero or more objects to format.</param>
        [Conditional("TRACE")]
        public static void WriteLine(string message, params object[] args)
        {
            try
            {
                MethodBase mb = GetCallingMethod();
                if (mb == null || config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                WriteLine(mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name, String.Format(message, args));
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e, EventLogName);
            }
        }

        /// <summary>
        /// Writes a prologue message
        /// </summary>
        [Conditional("TRACE")]
        public static void WriteMemberEntry()
        {
            try
            {
                MethodBase mb = GetCallingMethod();
                if (mb != null)
                {
                    if (config.excludedClasses.Contains(mb.DeclaringType.FullName))
                    {
                        return;
                    }
                    InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));


                    InternalWrite("{{{0}}} Entered {1}::{2}.{3}()", Thread.CurrentThread.ManagedThreadId.ToString(), mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name);
                    InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e, EventLogName);
            }
        }

        /// <summary>
        /// Writes a prologue message, listing arguments and values
        /// </summary>
        /// <param name="args">An rectangular array of name-value pairs</param>
        [Conditional("TRACE")]
        public static void WriteMemberEntry(object[,] args)
        {
            try
            {
                string detailedMessage = @"The args array must be a two-dimensional array with the 
                                       names in column 0 and the values in column 1. This could be 
                                       achieved with code which looks like:\n\n 
                                         args = new object[,] {
                                            {[arg1Name], [arg1Value]}, 
                                            {[arg2Name], [arg2Value]}, 
                                            {[arg3Name], [arg3Value]}, 
                                            {[arg4Name], [arg4Value]});";
                Debug.Assert(args.Rank == 2, "Rank of args must be 2", detailedMessage);
                Debug.Assert(args.GetLength(1) == 2, "args must have only two columns", detailedMessage);

                MethodBase mb = GetCallingMethod();
                if (mb == null || config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
                InternalWrite("{{{0}}} Entered {1}::{2}.{3}()", Thread.CurrentThread.ManagedThreadId.ToString(), mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name);
                for (int i = 0; i < args.GetLength(0); ++i)
                {
                    string name = args[i, 0].ToString();
                    string value = args[i, 1].ToString();
                    InternalWrite("{{{0}}}     {1} = {2}", Thread.CurrentThread.ManagedThreadId.ToString(), name, value);
                }
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e, EventLogName);
            }
        }

        /// <summary>
        /// Writes an epilogue message
        /// </summary>
        [Conditional("TRACE")]
        public static void WriteMemberExit()
        {
            try
            {
                MethodBase mb = GetCallingMethod();
                if (mb != null && mb.DeclaringType != null)
                {
                    if (config != null && config.excludedClasses != null && config.excludedClasses.Contains(mb.DeclaringType.FullName))
                    {
                        return;
                    }
                    InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
                    InternalWrite("{{{0}}} Exiting {1}::{2}.{3}()", Thread.CurrentThread.ManagedThreadId.ToString(), mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name);
                    InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e, EventLogName);
            }
        }

        /// <summary>
        /// Writes an epilogue message with the return value
        /// </summary>
        /// <param name="returnValue">The return value to log</param>
        [Conditional("TRACE")]
        public static void WriteMemberExit(object returnValue)
        {
            try
            {
                MethodBase mb = GetCallingMethod();
                if (mb == null || mb.DeclaringType == null || config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
                InternalWrite("{{{0}}} Exiting {1}::{2}.{3}()", Thread.CurrentThread.ManagedThreadId.ToString(), mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name);
                InternalWrite("{{{0}}} Return value is {1}", Thread.CurrentThread.ManagedThreadId.ToString(), returnValue.ToString());
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e, EventLogName);
            }
        }

        /// <summary>
        /// Standardised tracing of property gets/sets
        /// </summary>
        /// <param name="o">The value to be traced out</param>
        /// <remarks>Expected usage is:
        /// <code>
        /// internal static int SomeProperty {
        ///   get {
        ///     Trace.WritePropertyAccess(someProperty);
        ///     return someProperty;
        ///   }
        ///   set {
        ///     Trace.WritePropertyAccess(value);
        ///     someProperty = value;
        ///   }
        /// }
        /// </code>
        /// </remarks>
        [Conditional("TRACE")]
        public static void WritePropertyAccess(object o)
        {
            try
            {
                MethodBase mb = GetCallingMethod();
                if (mb == null || config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                string formatString = mb.Name.StartsWith("get_") ? "{{{0}}} Property get: {1}::{2}.{3} has value {4}" : "{{{0}}} Property set: {1}::{2}.{3} set to value {4}";
                InternalWrite(formatString, Thread.CurrentThread.ManagedThreadId.ToString(), mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name.Substring(4), o == null ? "null" : o.ToString());
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e, EventLogName);
            }
        }

        /// <summary>
        /// Standardised tracing of exceptions
        /// </summary>
        /// <param name="e">The exception to be traced out</param>
        /// <remarks>Expected usage is:
        /// <code>
        /// catch(Exception ex) {
        ///     FileTrace.WriteException(ex);
        ///     throw;
        /// }
        /// </code>
        /// </remarks>
        public static void WriteException(Exception e)
        {
            var dateTimeStamp = "Exception at: " + DateTime.Now.ToString();
            WriteException(e, dateTimeStamp);
        }

        /// <summary>
        /// Standardised tracing of exceptions
        /// </summary>
        /// <param name="e">The exception to be traced out</param>
        /// <remarks>Expected usage is:
        /// <code>
        /// catch(Exception ex) {
        ///     FileTrace.WriteException(ex);
        ///     throw;
        /// }
        /// </code>
        /// </remarks>
        public static void WriteException(Exception e, string additionalMessage)
        {
            try
            {
                MethodBase mb = e.TargetSite;
                int tid = Thread.CurrentThread.ManagedThreadId;
                InternalWrite(new String(config.exceptionDelimiter, config.exceptionDelimiterCount));

                if (e.StackTrace != null)
                {

                    InternalWrite("{{{0}}} Caught {1} thrown by {2}::{3}.{4}()", tid, e.GetType().Name, mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name);
                    InternalWrite("{{{0}}} {1}", tid, e.Message);
                    InternalWrite("{{{0}}} {1}", tid, e.StackTrace);
                }
                else
                {
                    InternalWrite("{{{0}}} Caught non-stack traceable exception. Error is: {1}", tid, e.Message);
                }


                if (additionalMessage != null)
                {
                    InternalWrite("{{{0}}} Additional information: {1}", tid, additionalMessage);
                }

                if (e.InnerException != null)
                {
                    InternalWrite("{{{0}}} There was a nested exception:", tid);
                    WriteException(e.InnerException);
                }
                else
                {
                    InternalWrite(new String(config.exceptionDelimiter, config.exceptionDelimiterCount));
                }
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "My Clinic";

                    // Exception.ToString does all the work for us - it
                    // prints out the message and the stack trace, and then
                    // recursively shows any nested exceptions. So there's
                    // no need for anything more than this:
                    eventLog.WriteEntry("My Clinic threw an exception:\n" + e, EventLogEntryType.Error);
                }
            }
            catch (Exception e2)
            {
                // Log a basic view of the error
                try
                {
                    if (e != null && e.Message != null)
                    {
                        var stackTrace = e.StackTrace;
                        if (stackTrace == null)
                            stackTrace = string.Empty;

                        int t = Thread.CurrentThread.ManagedThreadId;
                        InternalWrite("{{{0}}} {1}", t.ToString(), e.Message + " in ##### " + stackTrace);
                        InternalWrite(new String(config.exceptionDelimiter, config.exceptionDelimiterCount));
                    }
                    EventLogger.WriteLog(e2, EventLogName);
                }
                catch (Exception) { }
            }
        }
    }
}