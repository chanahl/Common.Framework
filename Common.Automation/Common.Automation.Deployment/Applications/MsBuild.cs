// <copyright file="MsBuild.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.Diagnostics;
using System.IO;
using Common.Automation.Deployment.Info;
using Common.Framework.Core.Enums;
using Common.Framework.Core.Extensions;

namespace Common.Automation.Deployment.Applications
{
    public abstract class MsBuild<TDeploymentInfo> : Application<TDeploymentInfo>
        where TDeploymentInfo : DeploymentInfo
    {
        protected MsBuild(
            MsBuildToolsVersion msBuildToolsVersion,
            TDeploymentInfo deploymentInfo)
            : base(deploymentInfo)
        {
            MsBuildToolsVersion = msBuildToolsVersion;

            ProcessStartInfo = new ProcessStartInfo(
                MsBuildExe.FullName,
                string.Format("\"{0}\" {1}", DeploymentInfo.Item, MsBuildSwitchParameters))
            {
                CreateNoWindow = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            if (!DeploymentInfo.IsInteractive)
            {
                DeploymentInfo.CommandFile.EnsureDirectory();
                var command = string.Format(
                    "\"{0}\" \"{1}\" {2}",
                    MsBuildExe.FullName,
                    DeploymentInfo.Item,
                    MsBuildSwitchParameters);
                using (var streamWriter = new StreamWriter(DeploymentInfo.CommandFile))
                {
                    streamWriter.WriteLine(command);
                }
            }
        }

        protected MsBuildToolsVersion MsBuildToolsVersion { get; private set; }

        protected FileInfo MsBuildExe
        {
            get { return MsBuildToolsVersion.GetMsBuildExe(); }
        }

        protected string MsBuildSwitchParameters
        {
            get { return GetMsBuildSwitchParameters(); }
        }

        protected abstract FileInfo GetMsBuildProject();

        protected abstract string GetMsBuildSwitchParameters();
    }
}