// <copyright file="Application.cs">
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
    public abstract class Application<TDeploymentInfo> where TDeploymentInfo : DeploymentInfo
    {
        protected Application(TDeploymentInfo deploymentInfo)
        {
            DeploymentInfo = deploymentInfo;
        }

        protected TDeploymentInfo DeploymentInfo { get; private set; }

        protected ProcessStartInfo ProcessStartInfo { get; set; }
        
        public bool Deploy()
        {
            try
            {
                if (PreExecuteOperation())
                {
                    return ExecuteOperation();
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return false;
        }

        protected virtual bool PreExecuteOperation()
        {
            return true;
        }

        protected virtual bool ExecuteOperation()
        {
            var processName = Path.GetFileName(DeploymentInfo.Item.ToString());
            bool processStatus;
            if (DeploymentInfo.IsInteractive)
            {
                var process = new Core.System.Process(ProcessStartInfo, processName);
                processStatus = process.Start(DeploymentInfo.TimeoutInMinutes);
            }
            else
            {
                var logFile = DeploymentInfo.LogFile;
                logFile.EnsureDirectory();

                var process = new Core.System.Process(ProcessStartInfo, processName, logFile);
                processStatus = process.Start(DeploymentInfo.TimeoutInMinutes);
            }

            return processStatus;
        }
    }
}