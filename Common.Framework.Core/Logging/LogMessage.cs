// <copyright file="LogMessage.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using Common.Framework.Core.Enums;

namespace Common.Framework.Core.Logging
{
    public class LogMessage
    {
        public LogMessage(
            DateTime logDateTime,
            LogSeverity logSeverity,
            string logSource,
            string logMessage)
        {
            DateTime = logDateTime;
            Severity = logSeverity;
            Source = logSource;
            Message = logMessage;
        }

        public DateTime DateTime { get; private set; }

        public LogSeverity Severity { get; private set; }

        public string Source { get; private set; }

        public string Message { get; private set; }
    }
}