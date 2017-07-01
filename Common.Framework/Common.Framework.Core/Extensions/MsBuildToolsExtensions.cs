// <copyright file="MsBuildToolsExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.IO;
using Common.Framework.Core.Enums;
using Microsoft.Win32;

namespace Common.Framework.Core.Extensions
{
    public static class MsBuildToolsExtensions
    {
        public static FileInfo GetMsBuildExe(this MsBuildToolsVersion msBuildToolsVersion)
        {
            var toolsVersion = msBuildToolsVersion.GetDescription();
            var keyName = string.Format(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\{0}\",
                toolsVersion);
            var directory = Registry.GetValue(
                keyName, "MSBuildToolsPath", null);
            return new FileInfo(Path.Combine(directory.ToString(), "MSBuild.exe"));
        }
    }
}