using System;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;

using System.Configuration;

using Utility.Toolkit.Configuration;

namespace Utility.Core.Logging
{
    public class EventLogger
    {
        private static string _eventLogName = CoreConfiguration.Instance.GetConfigValue("EventLogName");
        private static string _sourceName = "Application";

        public static void WriteLog(string logItem)
        {
            try
            {
                EventLog eventLog = CreateSource();
                eventLog.WriteEntry(logItem);
            }
            catch (InvalidOperationException)
            {
                Trace.WriteLine("Exception writing to log - possible registry access error");
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception)
            {
                
            }
        }

        public static void WriteLog(Exception e)
        {
            WriteLog(e, EventLogEntryType.Information);
        }

        public static void WriteLog(string m, EventLogEntryType t)
        {
            try
            {
                if (m != null && m.Trim().Length > 0)
                {
                    try
                    {
                        // Log the message to the event log
                        EventLog.WriteEntry(_sourceName, m, t);
                    }
                    catch (ThreadAbortException)
                    {
                        return;
                    }
                    catch (InvalidOperationException)
                    {
                        Trace.WriteLine("Exception writing to log - possible registry access error");
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error trying to write exception: " + ex.ToString() + " Exception raised during write: " + ex.ToString());
            }
        }

        public static void WriteLog(Exception e, EventLogEntryType t)
        {
            try
            {
                string message = string.Empty;
                if (e != null)
                {
                    try
                    {
                        if (e.Message != null)
                            message = message + "\nMessage : " + e.Message;

                        if (e.InnerException != null)
                        {
                            try
                            {
                                message = message + "\nInnerException : " + e.InnerException.Message + "\nInner Stack : " + e.InnerException.StackTrace;
                            }
                            catch (Exception parseException)
                            {
                                message = message + "\n ParseException : " + parseException.Message;
                                Trace.WriteLine(parseException.ToString());
                            }
                        }

                        if (e.Source != null)
                            message = message + "\nSource : " + e.Source;
                        if (e.StackTrace != null)
                            message = message + "\nStack : " + e.StackTrace;
                        if (e.TargetSite != null)
                            message = message + "\nTargetStite : " + e.TargetSite;

                        // Log the error to the event log
                        if (message.Trim() != string.Empty)
                        {
                            EventLog.WriteEntry(_sourceName, message, EventLogEntryType.Error);
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        return;
                    }
                    catch (InvalidOperationException)
                    {
                        Trace.WriteLine("Exception writing to log - possible registry access error");
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error trying to write exception: " + e.ToString() + " Exception raised during write: " + ex.ToString());
            }
        }

        public static void WriteLog(Exception e, string source)
        {
            string message = string.Empty;
            if (e != null)
            {
                try
                {
                    if (e.Message != null)
                        message = message + "\nMessage : " + e.Message;

                    if (e.InnerException != null)
                    {
                        try
                        {
                            message = message + "\nInnerException : " + e.InnerException.Message + "\nInner Stack : " + e.InnerException.StackTrace;
                        }
                        catch (Exception parseException)
                        {
                            message = message + "\n ParseException : " + parseException.Message;
                            Trace.WriteLine(parseException.ToString());
                        }
                    }

                    if (e.Source != null)
                        message = message + "\nSource : " + e.Source;
                    if (e.StackTrace != null)
                        message = message + "\nStack : " + e.StackTrace;
                    if (e.TargetSite != null)
                        message = message + "\nTargetStite : " + e.TargetSite;

                    // Log the error to the application event log
                    if (message.Trim() != string.Empty)
                    {
                        System.Diagnostics.EventLog eventLog = new EventLog("Application");
                        eventLog.Source = source;
                        eventLog.WriteEntry(message);
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (InvalidOperationException)
                {
                    Trace.WriteLine("Exception writing to log - possible registry access error");
                }
                catch (Exception)
                {
                }
            }
        }

        public static EventLog CreateSource()
        {
            EventLog hoksoftLog = null;
            try
            {
                string eventLogName = _eventLogName;
                string sourceName = _sourceName;

                hoksoftLog = new EventLog();
                hoksoftLog.Log = eventLogName;

                // set default event source (to be same as event log name) if not passed in
                if ((sourceName == null) || (sourceName.Trim().Length == 0))
                {
                    sourceName = eventLogName;
                }

                hoksoftLog.Source = sourceName;

                // Extra Raw event data can be added (later) if needed
                byte[] rawEventData = Encoding.ASCII.GetBytes("");

                /// Check whether the Event Source exists. It is possible that this may
                /// raise a security exception if the current process account doesn't
                /// have permissions for all sub-keys under
                /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services\EventLog

                // Check whether registry key for source exists

                string keyName = @"SYSTEM\CurrentControlSet\Services\EventLog\" + eventLogName;

                RegistryKey rkEventSource = Registry.LocalMachine.OpenSubKey(keyName + @"\" + sourceName);

                // Check whether key exists
                if (rkEventSource == null)
                {
                    /// Key does not exist. Create key which represents source
                    Registry.LocalMachine.CreateSubKey(keyName + @"\" + sourceName);
                }

                /// Now validate that the .NET Event Message File, EventMessageFile.dll (which correctly
                /// formats the content in a Log Message) is set for the event source
                object eventMessageFile = null;

                try
                {
                    rkEventSource.GetValue("EventMessageFile");
                }
                catch (Exception)
                { }

                /// If the event Source Message File is not set, then set the Event Source message file.
                if (eventMessageFile == null)
                {
                    /// Source Event File Doesn't exist - determine .NET framework location,
                    /// for Event Messages file.
                    RegistryKey dotNetFrameworkSettings = Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\Microsoft\.NetFramework\");

                    if (dotNetFrameworkSettings != null)
                    {
                        object dotNetInstallRoot = dotNetFrameworkSettings.GetValue("InstallRoot", null, RegistryValueOptions.None);
                        if (dotNetInstallRoot != null)
                        {
                            string eventMessageFileLocation = dotNetInstallRoot.ToString() + "v" + System.Environment.Version.Major.ToString() + "."
                                                                + System.Environment.Version.Minor.ToString() + "." + System.Environment.Version.Build.ToString() + @"\EventLogMessages.dll";

                            /// Validate File exists
                            if (System.IO.File.Exists(eventMessageFileLocation))
                            {
                                /// The Event Message File exists in the anticipated location on the
                                /// machine. Set this value for the new Event Source

                                // Re-open the key as writable
                                rkEventSource = Registry.LocalMachine.OpenSubKey(keyName + @"\" + sourceName, true);

                                // Set the "EventMessageFile" property
                                rkEventSource.SetValue("EventMessageFile", eventMessageFileLocation, RegistryValueKind.String);
                            }
                        }
                    }
                    dotNetFrameworkSettings.Close();
                }

                rkEventSource.Close();
            }
            catch (Exception)
            {
                
            }

            return hoksoftLog;
        }
    }
}