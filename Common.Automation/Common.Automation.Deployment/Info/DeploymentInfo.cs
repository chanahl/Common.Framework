// <copyright file="DeploymentInfo.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Automation.Deployment.Info
{
    public class DeploymentInfo
    {
        public bool IsInteractive { get; set; }

        public DeploymentType DeploymentType { get; set; }

        public string CommandFile { get; set; }

        public object Item { get; set; }

        public string LogFile { get; set; }

        public int TimeoutInMinutes { get; set; }
    }
}