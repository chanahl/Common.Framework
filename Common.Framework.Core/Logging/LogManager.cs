// <copyright file="LogManager.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Diagnostics;
using Common.Framework.Core.Enums;
using Common.Framework.Core.Patterns;

namespace Common.Framework.Core.Logging
{
    public sealed class LogManager : Subject
    {
        private static LogManager _logManagerInstance;

        private LogManager()
        {
        }

        public LogMessage LogMessage { get; set; }

        public static LogManager Instance()
        {
            if (_logManagerInstance == null)
            {
                _logManagerInstance = new LogManager();
            }

            return _logManagerInstance;
        }

        public static void RegisterLogFile(string logFileName)
        {
            var logFile = new LogFile(logFileName);
            logFile.Initialize();
            Instance().Attach(logFile);
        }

        public void LogDebugMessage(string debugMessage)
        {
            CreateLogMessage(LogSeverity.Debug, debugMessage);
        }

        public void LogErrorMessage(string errorMessage)
        {
            CreateLogMessage(LogSeverity.Error, errorMessage);
        }

        public void LogInfoMessage(string infoMessage)
        {
            CreateLogMessage(LogSeverity.Info, infoMessage);
        }

        public void LogResultMessage(string resultMessage)
        {
            CreateLogMessage(LogSeverity.Result, resultMessage);
        }

        public void LogTimeMessage(string timeMessage)
        {
            CreateLogMessage(LogSeverity.Time, timeMessage);
        }

        public void LogWarningMessage(string warningMessage)
        {
            CreateLogMessage(LogSeverity.Warning, warningMessage);
        }

        private static string GetLogSource()
        {
            try
            {
                var stackTrace = new StackTrace();
                var stackFrame = stackTrace.GetFrame(3);
                var methodBase = stackFrame.GetMethod();

                // ReSharper disable once PossibleNullReferenceException
                return methodBase.DeclaringType.Name + "." + methodBase.Name;
            }
            catch (Exception e)
            {
                var errorMessage = "Exception: " + e.Message;
                Instance().LogErrorMessage(errorMessage);
                throw new ApplicationException(errorMessage);
            }
        }

        private void CreateLogMessage(LogSeverity logSeverity, string logMessage)
        {
            var logDateTime = DateTime.Now;
            var logSource = GetLogSource();
            LogMessage = new LogMessage(logDateTime, logSeverity, logSource, logMessage);
            Notify();
        }
    }
}