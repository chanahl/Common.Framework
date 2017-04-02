// <copyright file="SqlServerApplication.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Diagnostics;
using System.IO;
using Common.Framework.Core.Extensions;
using Common.Framework.Deployment.Info;

namespace Common.Framework.Deployment.Applications
{
    public abstract class SqlServerApplication<TDeploymentInfo> : Application<TDeploymentInfo>
        where TDeploymentInfo : DeploymentInfo
    {
        protected SqlServerApplication(TDeploymentInfo deploymentInfo) : base(deploymentInfo)
        {
            if (!SqlApplication.Exists)
            {
                var errorMessage = string.Format("Could not locate SQL Application [{0}].", SqlApplication.FullName);
                throw new ArgumentException(errorMessage);
            }

            ProcessStartInfo = new ProcessStartInfo(
                SqlApplication.FullName,
                SqlApplicationParameters)
            {
                CreateNoWindow = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            if (!DeploymentInfo.IsInteractive)
            {
                DeploymentInfo.CommandFile.EnsureDirectory();
                var command = string.Format("\"{0}\" {1}", SqlApplication.FullName, SqlApplicationParameters);
                using (var streamWriter = new StreamWriter(DeploymentInfo.CommandFile))
                {
                    streamWriter.WriteLine(command);
                }
            }
        }

        protected FileInfo SqlApplication
        {
            get { return GetSqlApplication(); }
        }

        protected string SqlApplicationParameters
        {
            get { return GetSqlApplicationParameters(); }
        }

        protected abstract FileInfo GetSqlApplication();

        protected abstract string GetSqlApplicationParameters();
    }
}