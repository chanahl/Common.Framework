// <copyright file="SqlDatabaseDeploymentInfo.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Framework.Deployment.Info
{
    public class SqlDatabaseDeploymentInfo : DeploymentInfo
    {
        public SqlDatabaseDeploymentInfo(
            string targetUsername,
            string targetPassword)
        {
            TargetUsername = targetUsername;

            TargetPassword = targetPassword;
        }

        public string TargetUsername { get; private set; }

        public string TargetPassword { get; private set; }

        public string TargetServerName { get; set; }

        public string TargetDatabaseName { get; set; }

        public string SqlCmdVariables { get; set; }

        public bool DoCreateNewDatabase { get; set; }

        public bool DoDeployDatabaseInSingleUserMode { get; set; }
    }
}