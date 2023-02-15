using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Utility.Toolkit.Configuration;

namespace Utility.Core.Logging
{
    public class FileLogger
    {
        #region Fields

        private static FileLogger _instance = new FileLogger();

        private string _processId = Process.GetCurrentProcess().Id.ToString();
        private StreamWriter _logWriter;
        private FileStream _logStream;

        #endregion

        #region Public Properties

        public static FileLogger Instance
        {
            get { return _instance; }
        }

        #endregion

        #region Ctor

        private FileLogger()
        {
        }

        #endregion

        public void LogMessage(string message)
        {
            try
            {
                if (CoreConfiguration.Instance != null && CoreConfiguration.Instance.EnableLogging)
                {
                    CreateAppendLogFile();

                    _logWriter.WriteLine("{" + _processId + "} " + DateTime.Now + ": " + message);
                    CloseLogFile();
                }
            }
            catch (Exception) { }
        }

        private bool PurgeLog()
        {
            bool purge = false;
            try
            {
                string logFilePath = CoreConfiguration.Instance.LogPath;
                bool exists = File.Exists(logFilePath);

                if (exists)
                {
                    FileInfo fileInfo = new FileInfo(logFilePath);
                    long fileSize = fileInfo.Length;

                    if (fileSize > CoreConfiguration.Instance.MaxLogFileSize)
                    {
                        purge = true;
                    }
                }
            }
            catch (Exception) { }
            return purge;
        }

        private void CloseLogFile()
        {
            try
            {
                if (CoreConfiguration.Instance.EnableLogging && _logStream != null)
                {
                    _logWriter.Flush();
                    _logStream.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        private void CreateAppendLogFile()
        {
            try
            {
                bool logInformation = CoreConfiguration.Instance.EnableLogging;

                if (logInformation)
                {
                    bool needsPurging = PurgeLog();
                    if (needsPurging)
                    {
                        try
                        {
                            File.Delete(CoreConfiguration.Instance.LogPath);
                        }
                        catch (Exception)
                        { }
                    }

                    _logStream = new FileStream(CoreConfiguration.Instance.LogPath, FileMode.Append, FileAccess.Write, FileShare.Read, 4096, false);
                    _logWriter = new StreamWriter(_logStream, Encoding.UTF8);

                    _logWriter.AutoFlush = true;
                }
            }
            catch (Exception) { }
        }
    }
}