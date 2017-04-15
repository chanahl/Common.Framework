// <copyright file="SqlIspacDeploymentInfo.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Automation.Deployment.Info
{
    public class SqlIspacDeploymentInfo : DeploymentInfo
    {
        public SqlIspacDeploymentInfo(string sourceType = "File")
        {
            SourceType = sourceType;
        }

        public string SourceType { get; private set; }

        public string DestinationServer { get; set; }

        public string DestinationPath { get; set; }
    }
}