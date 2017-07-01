// <copyright file="AppConfigManager.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using Common.Framework.Core.Patterns;

namespace Common.Framework.Core.AppConfig
{
    public class AppConfigManager : Singleton<AppConfigManager, AppConfig>
    {
    }
}