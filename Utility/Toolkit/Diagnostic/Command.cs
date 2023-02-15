using System;
using Utility.Core.Logging;

namespace Utility.Toolkit.Diagnostic
{
    public class Command
    {
        public static string ExecuteCommandSync(string command)
        {
            return ExecuteCommandSync(command, false, 0);
        }

        public static string ExecuteCommandSync(string command, bool waitForExit)
        {
            return ExecuteCommandSync(command, waitForExit, 60000);
        }

        public static string ExecuteCommandSync(string command, bool waitForExit, int timeToWaitForExit)
        {
            FileTrace.WriteMemberEntry();

            string commandResult = string.Empty;
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;

                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;

                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                Trace.WriteLine("About to wait for exit: " + timeToWaitForExit + " seconds");
                if (timeToWaitForExit == 0)
                {
                    proc.WaitForExit();
                }
                else
                {
                    bool finished = proc.WaitForExit(timeToWaitForExit);
                    Trace.WriteLine("finished is: " + finished);
                }

                // Get the output into a string
                commandResult = proc.StandardOutput.ReadToEnd();
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }
            Trace.WriteLine("commandResult is: " + commandResult);

            FileTrace.WriteMemberExit();
            return commandResult;
        }

        public static string ExecuteBatchSync(string batchFilePath)
        {
            return ExecuteBatchSync(batchFilePath, false, 0);
        }

        public static string ExecuteBatchSync(string batchFilePath, bool waitForExit)
        {
            return ExecuteBatchSync(batchFilePath, waitForExit, 60000);
        }

        public static string ExecuteBatchSync(string batchFilePath, bool waitForExit, int timeToWaitForExit)
        {
            FileTrace.WriteMemberEntry();

            string commandResult = string.Empty;
            try
            {
                System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo(batchFilePath);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                //procStartInfo.RedirectStandardOutput = false;
                //procStartInfo.UseShellExecute = true;

                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;

                // Now we create a process, assign its ProcessStartInfo and start it
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                if (waitForExit)
                {
                    Trace.WriteLine("About to wait for exit: " + timeToWaitForExit + " seconds");
                    if (timeToWaitForExit == 0)
                    {
                        proc.WaitForExit();
                    }
                    else
                    {
                        bool finished = proc.WaitForExit(timeToWaitForExit);
                        Trace.WriteLine("finished is: " + finished);
                    }
                }
            }
            catch (Exception e)
            {
                FileTrace.WriteException(e);
            }
            Trace.WriteLine("commandResult is: " + commandResult);

            FileTrace.WriteMemberExit();
            return commandResult;
        }
    }
}
