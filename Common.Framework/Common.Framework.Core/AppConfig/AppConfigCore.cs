// <copyright file="AppConfigCore.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using Common.Framework.Core.Enums;
using Common.Framework.Core.Extensions;
using Common.Framework.Core.Logging;

namespace Common.Framework.Core.AppConfig
{
    public class AppConfigCore
    {
        public AppConfigCore()
        {
            Initialize();
        }

        public string ApplicationName { get; set; }

        public bool IsDebugMode { get; set; }

        public bool IsLogSuppressed { get; set; }

        public string LogDirectory { get; set; }

        public string RunType { get; set; }

        public string TargetDirectory { get; set; }

        public void LogCore()
        {
            LogManager.Instance().LogInfoMessage(typeof(AppConfigCore).Name.Separate() + ":");

            foreach (var appConfigProperty in GetType().GetProperties())
            {
                if (appConfigProperty.Name.Equals(Constants.AppConfigParametersSection))
                {
                    continue;
                }

                var type = GetType(appConfigProperty.PropertyType);
                var name = appConfigProperty.Name.Separate();
                switch (type.Name)
                {
                    case Constants.TypeNameDictionary:
                        var dictionary = (Dictionary<string, string>)appConfigProperty.GetValue(this, null);
                        foreach (var item in dictionary)
                        {
                            LogManager.Instance().LogInfoMessage(
                                string.Format("    {0}: [{1}, {2}]", name, item.Key, item.Value));
                        }

                        if (dictionary.Count.Equals(0))
                        {
                            LogManager.Instance().LogInfoMessage(
                                string.Format("    {0}: [{1},{2}]", name, null, null));
                        }

                        break;
                    case Constants.TypeNameList:
                        var list = (List<string>)appConfigProperty.GetValue(this, null);
                        foreach (var item in list)
                        {
                            LogManager.Instance().LogInfoMessage(
                                string.Format("    {0}: [{1}]", name, item));
                        }

                        if (list.Count.Equals(0))
                        {
                            LogManager.Instance().LogInfoMessage(
                                string.Format("    {0}: [{1}]", name, null));
                        }

                        break;
                    case Constants.TypeNameString:
                        LogManager.Instance().LogInfoMessage(
                            string.Format(
                                "    {0}: [{1}]", name, appConfigProperty.GetValue(this)));
                        break;
                }
            }
        }

        private static Type GetType(Type type)
        {
            if (typeof(Dictionary<string, string>).IsAssignableFrom(type))
            {
                return typeof(Dictionary<string, string>);
            }

            if (typeof(List<string>).IsAssignableFrom(type))
            {
                return typeof(List<string>);
            }

            return typeof(string);
        }

        private void Initialize()
        {
            try
            {
                var appConfigCore = ConfigurationManager.GetSection(Constants.AppConfigCoreSection) as NameValueCollection;
                if (appConfigCore != null)
                {
                    ApplicationName = string.IsNullOrEmpty(appConfigCore[Constants.ApplicationName])
                        ? "Not Provided"
                        : appConfigCore[Constants.ApplicationName];

                    IsDebugMode =
                        !string.IsNullOrEmpty(appConfigCore[Constants.IsDebugMode]) &&
                        Convert.ToBoolean(appConfigCore[Constants.IsDebugMode]);

                    IsLogSuppressed =
                        !string.IsNullOrEmpty(appConfigCore[Constants.IsLogSuppressed]) &&
                        Convert.ToBoolean(appConfigCore[Constants.IsLogSuppressed]);

                    LogDirectory = string.IsNullOrEmpty(appConfigCore[Constants.LogDirectory])
                        ? Path.Combine(Directory.GetCurrentDirectory(), "Logs")
                        : appConfigCore[Constants.LogDirectory];

                    RunType = string.IsNullOrEmpty(appConfigCore[Constants.RunType])
                        ? EnvironmentType.Development.GetDescription()
                        : appConfigCore[Constants.RunType];

                    TargetDirectory = string.IsNullOrEmpty(appConfigCore[Constants.TargetDirectory])
                        ? Directory.GetCurrentDirectory()
                        : Environment.ExpandEnvironmentVariables(appConfigCore[Constants.TargetDirectory]);
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }
}