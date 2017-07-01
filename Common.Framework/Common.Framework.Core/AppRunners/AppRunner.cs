// <copyright file="AppRunner.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Framework.Core.AppConfig;
using Common.Framework.Core.Enums;
using Common.Framework.Core.Extensions;
using Common.Framework.Core.Logging;
using Common.Framework.Core.Performance;

namespace Common.Framework.Core.AppRunners
{
    public abstract class AppRunner
    {
        private StopwatchManager StopwatchManager { get; set; }
        
        public void Run()
        {
            InitializeAppConfig();

            InitializeLogFile();

            LogAppConfig();

            if (Main())
            {
                End();
            }
            else
            {
                throw new ApplicationException("Could not run application.");
            }
        }

        protected abstract bool Main();

        protected virtual IEnumerable<string> GetCatalogue(
            AssemblyType assemblyType,
            string nameSpace)
        {
            var classes = new List<string>();
            var namespaceGroups = assemblyType.GetNamespaces();
            foreach (var namespaceGroup in namespaceGroups)
            {
                if (namespaceGroup == null)
                {
                    continue;
                }

                if (namespaceGroup.Contains(nameSpace) || namespaceGroup.Contains(nameSpace + "."))
                {
                    classes.AddRange(assemblyType.GetClasses(namespaceGroup));
                }
            }

            return classes;
        }

        protected virtual string GetLogFileName(
            string applicationName,
            string logDirectory)
        {
            var date = DateTime.Now.ToString(Constants.DateFormatIso8601);
            var runType = AppConfigManager.Instance().Property.AppConfigCore.RunType;
            var logFileName = Path.Combine(logDirectory, applicationName + "_" + date + "_" + runType + ".log");
            return logFileName;
        }

        protected T Instantiate<T>(
            AssemblyType assemblyType,
            string name,
            params object[] arguments)
        {
            try
            {
                return
                    arguments.Any()
                        ? (T)assemblyType.Instantiate(name, arguments)
                        : (T)assemblyType.Instantiate(name);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        private static void LogAppConfig()
        {
            LogManager.Instance().LogInfoMessage("App.config initialized successfully.");

            AppConfigManager.Instance().Property.AppConfigCore.LogCore();
            if (!AppConfigManager.Instance().Property.AppConfigCore.IsLogSuppressed)
            {
                AppConfigManager.Instance().Property.AppConfigParameters.LogParameters();
            }
        }

        private void InitializeAppConfig()
        {
            try
            {
                InitializeStopwatchManager();
            }
            catch (Exception e)
            {
                var errorMessage = "Exception: " + e.Message;
                throw new ApplicationException(errorMessage);
            }

            var appConfigManager = AppConfigManager.Instance();
            try
            {
                appConfigManager.InitializeProperty();
            }
            catch (Exception e)
            {
                var errorMessage = "Exception: " + e.Message;
                throw new ApplicationException(errorMessage);
            }
        }

        private void InitializeLogFile()
        {
            var logDirectory = AppConfigManager.Instance().Property.AppConfigCore.LogDirectory;
            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
            }
            catch (Exception e)
            {
                var errorMessage = e.Message;
                Console.WriteLine("Could not create log directory provided in App.config.");
                throw new ApplicationException(errorMessage);
            }

            var applicationName =
                string.IsNullOrEmpty(AppConfigManager.Instance().Property.AppConfigCore.ApplicationName)
                    ? AssemblyType.Entry.GetAssemblyName()
                    : AppConfigManager.Instance().Property.AppConfigCore.ApplicationName;
            var logFileName = GetLogFileName(applicationName, logDirectory);
            LogManager.RegisterLogFile(logFileName);
        }

        private void InitializeStopwatchManager()
        {
            StopwatchManager = StopwatchManager.Instance();
            StopwatchManager.WarmupAndStart();
        }

        private void End()
        {
            StopwatchManager.Stop();

            const string TimeMessage = "Amount of elapsed time is [";
            LogManager.Instance().LogTimeMessage(
                TimeMessage +
                StopwatchManager.Hours + ":" +
                StopwatchManager.Minutes + ":" +
                StopwatchManager.Seconds + "." +
                StopwatchManager.Milliseconds + "].");

            const string InfoMessage = "Application has completed successfully.";
            LogManager.Instance().LogInfoMessage(InfoMessage);
        }
    }
}