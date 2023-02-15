using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Utility.Toolkit.Diagnostic
{
    /// <summary>
    /// Provides a set of methods and properties that help you trace the 
    /// execution of your code. This class cannot be inherited.
    /// </summary>
    [ReflectionPermission(SecurityAction.Assert)]
    public sealed class Trace
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

        #region Fields

        private static readonly TraceConfig config;

        #endregion

        #region Ctor

        static Trace()
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

        #endregion

        #region Private Methods

        static void InternalWrite(string message, params object[] formatObjects)
        {
            try
            {
                // Format will be in:
                // string namespaceName, string typeName, string memberName, string message
                if (message != String.Empty)
                {
                    string m = String.Format(message, formatObjects);
                    System.Diagnostics.Trace.WriteLine(m);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e,   EventLogEntryType.Error);
            }
        }

        static MethodBase GetCallingMethod()
        {
            MethodBase mb = null;
            try
            {
                StackTrace st = new StackTrace();
                for (int i = 0; i < st.FrameCount; ++i)
                {
                    mb = st.GetFrame(i).GetMethod();
                    if (mb.DeclaringType.FullName != typeof(Trace).FullName)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e,   EventLogEntryType.Error);
            }
            return mb;
        }

        [Conditional("TRACE")]
        static void WriteLine(string namespaceName, string typeName, string memberName, string message)
        {
            try
            {
                InternalWrite(config.formatString, Thread.CurrentThread.ManagedThreadId, message, namespaceName, typeName, memberName, DateTime.Now.ToLongTimeString());
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e,  EventLogEntryType.Error);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes information about the trace to the trace listeners in the Listeners collection.
        /// </summary>
        /// <param name="className">The class from which the trace information is being 
        /// written.</param>
        /// <param name="methodName">The method from which the trace information is being 
        /// written.</param>
        /// <param name="message">A message to write.</param>
        [Obsolete("Use of Trace.WriteLine(className, methodName, message) is deprecated. Please use WriteLine(message) instead", false), Conditional("TRACE")]
        [Conditional("TRACE")]
        public static void WriteLine(string className, string methodName, string message)
        {
            try
            {
                MethodBase mb = GetCallingMethod();
                if (config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                InternalWrite("{" + Thread.CurrentThread.ManagedThreadId + "} " + message + " (" + className + "::" + methodName + ") @ " + DateTime.Now.ToUniversalTime());
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e,   EventLogEntryType.Error);
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
                if (config.excludedClasses.Contains(mb.DeclaringType.FullName))
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
                EventLogger.WriteLog(e, EventLogEntryType.Error);
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
                if (config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                try
                {
                    WriteLine(mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name, String.Format(message, args));
                }
                catch (Exception e)
                {
                    // Swallow the exception
                    Trace.WriteLine("Exception in diagnostics: " + e.Message + " Original error message: " + message);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e,   EventLogEntryType.Error);
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
                if (config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
                InternalWrite("{{{0}}} Entered {1}::{2}.{3}()", Thread.CurrentThread.ManagedThreadId, mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name);
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e,   EventLogEntryType.Error);
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
                if (config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
                InternalWrite("{{{0}}} Entered {1}::{2}.{3}()", Thread.CurrentThread.ManagedThreadId, mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name);
                for (int i = 0; i < args.GetLength(0); ++i)
                {
                    string name = args[i, 0].ToString();
                    string value = args[i, 1].ToString();
                    InternalWrite("{{{0}}}     {1} = {2}", Thread.CurrentThread.ManagedThreadId, name, value);
                }
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e,   EventLogEntryType.Error);
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
                if (config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
                InternalWrite("{{{0}}} Exiting {1}::{2}.{3}()", Thread.CurrentThread.ManagedThreadId, mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name);
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e,   EventLogEntryType.Error);
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
                if (config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
                InternalWrite("{{{0}}} Exiting {1}::{2}.{3}()", Thread.CurrentThread.ManagedThreadId, mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name);
                InternalWrite("{{{0}}} Return value is {1}", Thread.CurrentThread.ManagedThreadId, returnValue.ToString());
                InternalWrite(new String(config.functionDelimiter, config.functionDelimiterCount));
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e,   EventLogEntryType.Error);
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
                if (config.excludedClasses.Contains(mb.DeclaringType.FullName))
                {
                    return;
                }
                string formatString = mb.Name.StartsWith("get_") ? "{{{0}}} Property get: {1}::{2}.{3} has value {4}" : "{{{0}}} Property set: {1}::{2}.{3} set to value {4}";
                InternalWrite(formatString, Thread.CurrentThread.ManagedThreadId, mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.Name.Substring(4), o == null ? "null" : o.ToString());
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                EventLogger.WriteLog(e,   EventLogEntryType.Error);
            }
        }

        public static string WriteException(Exception e)
        {
            string s = string.Empty;
            try
            {
                s = WriteException(e, null, true);
            }
            catch (Exception)
            {
                // Surpress the error
            }
            return s;
        }

        public static string WriteException(Exception e, string additionalMessage)
        {
            string s = string.Empty;
            try
            {
                s = WriteException(e, additionalMessage, true);
            }
            catch (Exception)
            {
                // Surpress the error
            }
            return s;
        }

        /// <summary>
        /// Standardised tracing of exceptions
        /// </summary>
        /// <param name="e">The exception to be traced out</param>
        /// <remarks>Expected usage is:
        /// <code>
        /// catch(Exception ex) {
        ///     Trace.WriteException(ex);
        ///     throw;
        /// }
        /// </code>
        /// </remarks>
        public static string WriteException(Exception e, bool logToEventLog)
        {
            string s = string.Empty;
            try
            {
                s = WriteException(e, null, logToEventLog);
            }
            catch (Exception)
            {
                // Surpress the error
            }
            return s;
        }

        /// <summary>
        /// Standardised tracing of exceptions
        /// </summary>
        /// <param name="e">The exception to be traced out</param>
        /// <remarks>Expected usage is:
        /// <code>
        /// catch(Exception ex) {
        ///     Trace.WriteException(ex);
        ///     throw;
        /// }
        /// </code>
        /// </remarks>
        public static string WriteException(Exception e, string additionalMessage, bool logToEventLog)
        {
            StringBuilder s = new StringBuilder();
            try
            {
                MethodBase mb = e.TargetSite;
                int tid = Thread.CurrentThread.ManagedThreadId;

                s.Append(new string(config.exceptionDelimiter, config.exceptionDelimiterCount));
                InternalWrite(new string(config.exceptionDelimiter, config.exceptionDelimiterCount));

                s.Append(string.Format("{{{0}}} Caught {1} thrown by {2}::{3}.{4}()", tid, e.GetType().Name, mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name));
                InternalWrite("{{{0}}} Caught {1} thrown by {2}::{3}.{4}()", tid, e.GetType().Name, mb.DeclaringType.Namespace, mb.DeclaringType.Name, mb.IsConstructor ? mb.DeclaringType.Name : mb.Name);

                s.Append(string.Format("{{{0}}} {1}", tid, e.Message));
                InternalWrite("{{{0}}} {1}", tid, e.Message);

                s.Append(string.Format("{{{0}}} {1}", tid, e.StackTrace));
                InternalWrite("{{{0}}} {1}", tid, e.StackTrace);
                
                if (additionalMessage != null)
                {
                    s.Append(string.Format("{{{0}}} Additional information: {1}", tid, additionalMessage));
                    InternalWrite("{{{0}}} Additional information: {1}", tid, additionalMessage);
                }

                if (e.InnerException != null)
                {
                    s.Append(string.Format("{{{0}}} There was a nested exception:", tid));
                    InternalWrite("{{{0}}} There was a nested exception:", tid);
                    WriteException(e.InnerException, logToEventLog);
                }
                else
                {
                    s.Append(new string(config.exceptionDelimiter, config.exceptionDelimiterCount));
                    InternalWrite(new String(config.exceptionDelimiter, config.exceptionDelimiterCount));
                }

                if (logToEventLog)
                    EventLogger.WriteLog(e, EventLogEntryType.Error);
            }
            catch (Exception e2)
            {
                EventLogger.WriteLog(e2,   EventLogEntryType.Error);
            }

            return s.ToString();
        }

        /// <summary>
        /// Creates a string of all property value pair in the provided object instance
        /// </summary>
        /// <param name="objectToGetStateOf"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static string GetLogFor(object objectToGetStateOf)
        {
            if (objectToGetStateOf == null)
            {
                const string PARAMETER_NAME = "objectToGetStateOf";
                throw new ArgumentException(string.Format("Parameter {0} cannot be null", PARAMETER_NAME), PARAMETER_NAME);
            }
            var builder = new StringBuilder();

            foreach (var property in objectToGetStateOf.GetType().GetProperties())
            {
                object value = property.GetValue(objectToGetStateOf, null);

                builder.Append(property.Name)
                .Append(" = ")
                .Append((value ?? "null"))
                .AppendLine();
            }
            return builder.ToString();
        }

        #endregion
    }
}