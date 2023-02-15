using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

using RMS.Core.Utitlites.Configuration;

namespace RMS.Core.Utitlites.Diagnostic
{
    public class EventLogger
    {
        public static void WriteLog(string logItem)
        {
            WriteLog(logItem, EventLogEntryType.Information);
        }

        public static void WriteLog(string logItem, EventLogEntryType eventType)
        {
            try
            {
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = CoreConfiguration.Instance.EventLogSourceName;
                    eventLog.WriteEntry(logItem, eventType);
                }
            }
            catch (InvalidOperationException)
            {
                Trace.WriteLine("Exception writing to log - possible registry access error");
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                // Don't call Trace.WriteException - that ends up calling
                // back into WriteLog!
                Trace.WriteLine(e.ToString());
            }
        }
    }
}