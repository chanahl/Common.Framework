// <copyright file="AppConfig.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Framework.Core.AppConfig
{
    public class AppConfig
    {
        public AppConfig()
        {
            AppConfigCore = new AppConfigCore();
            AppConfigParameters = new AppConfigParameters();
        }

        public AppConfigCore AppConfigCore { get; set; }

        public AppConfigParameters AppConfigParameters { get; set; }
    }
}