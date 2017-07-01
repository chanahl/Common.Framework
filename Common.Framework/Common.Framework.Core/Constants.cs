// <copyright file="Constants.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Framework.Core
{
    public class Constants
    {
        #region App.config Configuration Sections

        public const string AppConfigCoreSection = "AppConfigCore";

        public const string AppConfigParametersSection = "AppConfigParameters";

        #endregion // App.config Configuration Sections

        #region App.config Core

        public const string ApplicationName = "ApplicationName";

        public const string IsDebugMode = "IsDebugMode";

        public const string IsLogSuppressed = "IsLogSuppressed";

        public const string LogDirectory = "LogDirectory";

        public const string RunType = "RunType";

        public const string TargetDirectory = "TargetDirectory";

        #endregion // App.config Core

        #region Date Time Formats

        public const string DateFormatGregorian = "dd MMM yyyy";

        public const string DateFormatIso8601 = "yyyyMMdd";

        public const string DateFormatIso8601Short = "yyMMdd";

        public const string DateTimeFormatIso8601 = "yyyy-MM-dd HH:mm:ss";

        #endregion // Date Time Formats

        #region Type Names

        public const string TypeNameDictionary = "Dictionary`2";

        public const string TypeNameList = "List`1";

        public const string TypeNameString = "String";

        #endregion // Type Names
    }
}