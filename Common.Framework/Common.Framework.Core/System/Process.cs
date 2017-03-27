// <copyright file="Process.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Common.Framework.Core.Logging;

namespace Common.Framework.Core.System
{
    public class Process
    {
        public Process(
            ProcessStartInfo processStartInfo,
            string processName)
        {
            ProcessStartInfo = processStartInfo;
            ProcessName = processName;
        }

        public Process(
            ProcessStartInfo processStartInfo,
            string processName,
            string logFileName)
        {
            ProcessStartInfo = processStartInfo;
            ProcessName = processName;
            LogFileName = logFileName;
        }

        public static ProcessStartInfo ProcessStartInfo { get; set; }

        public static string ProcessName { get; set; }

        public static string LogFileName { get; set; }

        public static bool AllTrue(bool trueOrFalse)
        {
            return trueOrFalse;
        }

        public static void ExecuteBatchScript(string fileName)
        {
            var process = new global::System.Diagnostics.Process
            {
                StartInfo = { FileName = fileName }
            };
            process.Start();
            process.WaitForExit();
        }

        public static void ExecuteCommandAsync(string command)
        {
            try
            {
                var thread = new Thread(ExecuteCommandSync)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.AboveNormal
                };

                thread.Start(command);
            }
            catch (ThreadStartException threadStartException)
            {
                Console.WriteLine(threadStartException.Message);
            }
            catch (ThreadAbortException threadAbortException)
            {
                Console.WriteLine(threadAbortException.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public static void ExecuteCommandSync(object command)
        {
            try
            {
                var processInfo =
                    new ProcessStartInfo("cmd.exe", "/c " + command)
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };

                var process = global::System.Diagnostics.Process.Start(processInfo);
                if (process == null)
                {
                    return;
                }

                process.WaitForExit();
                process.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static string ExecuteCommandSyncRedirectOutput(string command)
        {
            try
            {
                var processInfo =
                    new ProcessStartInfo("cmd.exe", "/c " + command)
                    {
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };

                var process = global::System.Diagnostics.Process.Start(processInfo);
                if (process == null)
                {
                    return string.Empty;
                }

                string output;
                using (var streamReader = process.StandardOutput)
                {
                    output = streamReader.ReadToEnd();
                }

                process.WaitForExit();
                process.Close();

                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return string.Empty;
        }

        public bool Start(int timeoutInMinutes = 0)
        {
            try
            {
                var process = global::System.Diagnostics.Process.Start(ProcessStartInfo);
                if (process == null)
                {
                    LogManager.Instance().LogErrorMessage("Could not start process.");
                    return false;
                }

                if (!string.IsNullOrEmpty(LogFileName))
                {
                    process.OutputDataReceived += LogToFile;
                    process.ErrorDataReceived += LogToFile;
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }

                if (timeoutInMinutes > 0)
                {
                    LogManager.Instance().LogInfoMessage(
                        "Timeout specified; will wait [" + timeoutInMinutes +
                        "] minutes for the associated process to exit.");
                    var waitForExit = process.WaitForExit(60000 * timeoutInMinutes);
                    if (!waitForExit)
                    {
                        LogManager.Instance().LogWarningMessage(
                            "Process exceeded timeout period of [" + timeoutInMinutes + "] minutes.");

                        try
                        {
                            process.CloseMainWindow();
                            LogManager.Instance().LogWarningMessage(
                                "Closed main window of process [" + process.ProcessName + "].");
                        }
                        catch (Exception exception)
                        {
                            LogManager.Instance().LogErrorMessage(exception.Message);
                        }

                        try
                        {
                            process.Kill();
                            LogManager.Instance().LogWarningMessage("Killed process [" + process.ProcessName + "].");
                        }
                        catch (Exception exception)
                        {
                            LogManager.Instance().LogErrorMessage(exception.Message);
                        }
                    }
                }
                else
                {
                    LogManager.Instance().LogInfoMessage(
                        "No timeout specified; will wait indefinitely for the associated process to exit.");
                    process.WaitForExit();
                }

                var exitCode = process.ExitCode;
                process.Close();

                if (!exitCode.Equals(0))
                {
                    LogManager.Instance().LogErrorMessage("Process exited with error code [" + exitCode + "].");
                    return false;
                }

                LogManager.Instance().LogInfoMessage("Process completed successfully for [" + ProcessName + "].");
                return true;
            }
            catch (Exception exception)
            {
                LogManager.Instance().LogErrorMessage(exception.Message);
                return false;
            }
        }

        private static void LogToFile(
            object sender,
            DataReceivedEventArgs e)
        {
            var logFileDirectory = Path.GetDirectoryName(LogFileName);
            if (logFileDirectory != null && !Directory.Exists(logFileDirectory))
            {
                Directory.CreateDirectory(logFileDirectory);
            }

            try
            {
                using (var fileStream = new FileStream(LogFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (var streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.WriteLine(e.Data);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.Instance().LogWarningMessage(exception.Message);
            }
        }
    }
}