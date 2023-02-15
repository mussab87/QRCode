using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Configuration;
using Utility.Toolkit.Diagnostic;
using Utility.Core.Logging;
using Utility.Toolkit.Security;

namespace Utility.Toolkit.Configuration
{
    public class CoreConfiguration
    {
        #region Fields

        private static CoreConfiguration _instance = new CoreConfiguration();

        private bool _enableLogging = false;
        private string _logPath = string.Empty;
        private int _maxLogFileSize = 100000000;
        private int _suspensionSwitch = 0;
        private string _eventLogSourceName = "Application";
        private static readonly string _secretKey = "Encrypti0n@K6y";

        #endregion

        #region Public Properties

        public static CoreConfiguration Instance
        {
            get { return _instance; }
        }

        public bool EnableLogging
        {
            get { return _enableLogging; }
        }

        public string LogPath
        {
            get { return _logPath; }
        }

        public int MaxLogFileSize
        {
            get { return _maxLogFileSize; }
        }

        public string EventLogSourceName
        {
            get { return _eventLogSourceName; }
        }


        public int SuspensionSwitch
        {
            get { return _suspensionSwitch; }
        }
        #endregion

        #region Ctor

        private CoreConfiguration()
        {
            FileTrace.WriteMemberEntry();
            try
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains("EventLogSourceName"))
                    _eventLogSourceName = ConfigurationManager.AppSettings["EventLogSourceName"];

                if (ConfigurationManager.AppSettings.AllKeys.Contains("EnableLogging"))
                    _enableLogging = Boolean.Parse(ConfigurationManager.AppSettings["EnableLogging"]);

                if (ConfigurationManager.AppSettings.AllKeys.Contains("LogFilePath"))
                    _logPath = ConfigurationManager.AppSettings["LogFilePath"];

                if (ConfigurationManager.AppSettings.AllKeys.Contains("MaxLogFileSize"))
                    _maxLogFileSize = Int32.Parse(ConfigurationManager.AppSettings["MaxLogFileSize"]);

                if (ConfigurationManager.AppSettings.AllKeys.Contains("SuspensionSwitch"))
                    _suspensionSwitch = Int32.Parse(ConfigurationManager.AppSettings["SuspensionSwitch"]);
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }
            FileTrace.WriteMemberExit();
        }

        #endregion

        #region Public Methods

        public string GetConfigValue(string configKey)
        {
            FileTrace.WriteMemberEntry();

            string configValue = string.Empty;
            try
            {
                configValue = ConfigurationManager.AppSettings[configKey];
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }

            FileTrace.WriteMemberExit();
            return configValue;
        }

        public string GetConnValue(string connName)
        {
            FileTrace.WriteMemberEntry();

            string configValue = string.Empty;
            try
            {
                configValue = GetConnValue(connName, false);
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }

            FileTrace.WriteMemberExit();
            return configValue;
        }

        public string GetConnValue(string connName, bool encrypted)
        {
            FileTrace.WriteMemberEntry();

            string configValue = string.Empty;
            try
            {
                // Get the connection string
                configValue = ConfigurationManager.ConnectionStrings[connName].ConnectionString;

                // If encrypted, decrypt.
                if (encrypted)
                {
                    configValue = Crypto.DecryptStringDES(configValue, _secretKey);
                }
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }

            FileTrace.WriteMemberExit();
            return configValue;
        }

        #endregion
    }

}