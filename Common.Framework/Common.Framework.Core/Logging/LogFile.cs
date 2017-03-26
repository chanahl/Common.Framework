// <copyright file="LogFile.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.IO;
using Common.Framework.Core.AppConfig;
using Common.Framework.Core.Enums;
using Common.Framework.Core.Extensions;
using Common.Framework.Core.Patterns;

namespace Common.Framework.Core.Logging
{
    public class LogFile : Observer
    {
        public LogFile(string logFileName)
        {
            LogFileName = logFileName;
        }

        public string LogFileName { get; private set; }

        public void Initialize()
        {
            var runDateTime = DateTime.Now;
            var machineName = Environment.MachineName;
            var operatingSystemVersion = Environment.OSVersion.VersionString;

            try
            {
                using (var logFile = new StreamWriter(LogFileName, true))
                {
                    logFile.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    logFile.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    logFile.WriteLine("Application Name [" + AssemblyType.Entry.GetAssemblyName() + "]");
                    logFile.WriteLine("Run Type [" + AppConfigManager.Instance().Property.AppConfigCore.RunType + "]");
                    logFile.WriteLine("Run Date [" + runDateTime.ToString(Constants.DateTimeFormatIso8601) + "]");
                    logFile.WriteLine("Machine Name [" + machineName + "]");
                    logFile.WriteLine("OS Version [" + operatingSystemVersion + "]");
                    logFile.WriteLine(">");
                }
            }
            catch (Exception e)
            {
                var errorMessage = "Exception: " + e.Message;
                LogManager.Instance().LogErrorMessage(errorMessage);
                throw new ApplicationException(errorMessage);
            }
        }

        public override void Update()
        {
            LogThisMessage(LogManager.Instance().LogMessage);
        }

        private void LogThisMessage(LogMessage logMessage)
        {
            if (logMessage.Severity.Equals(LogSeverity.Debug) &&
                 AppConfigManager.Instance().Property.AppConfigCore.RunType.Equals(EnvironmentType.Production.GetDescription()))
            {
                // do not log debug messages when run type is production
                return;
            }

            try
            {
                using (var logFile = new StreamWriter(LogFileName, true))
                {
                    var logDateTime =
                        logMessage.DateTime.ToString(Constants.DateTimeFormatIso8601);
                    logFile.WriteLine(
                        "[" + logDateTime + "]" +
                        "[" + logMessage.Severity.ToString().ToUpper() + "]" +
                        "[" + logMessage.Source + "] " +
                        logMessage.Message);
                }
            }
            catch (Exception e)
            {
                var errorMessage = "Exception: " + e.Message;
                LogManager.Instance().LogErrorMessage(errorMessage);
                throw new ApplicationException(errorMessage);
            }
        }
    }
}