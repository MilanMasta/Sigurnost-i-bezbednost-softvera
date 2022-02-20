using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ServiceApp
{
    public class Logger
    {
        public static void Log(string logMessage, EventLogEntryType logType)
        {
            string cs = "CertificateService";
            if (!EventLog.SourceExists(cs))
                EventLog.CreateEventSource(cs, "Application");

            EventLog.WriteEntry(cs, logMessage, logType);
        }
    }
}
