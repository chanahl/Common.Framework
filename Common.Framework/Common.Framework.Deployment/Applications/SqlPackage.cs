// <copyright file="SqlPackage.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.IO;
using Common.Framework.Deployment.Info;

namespace Common.Framework.Deployment.Applications
{
    public class SqlPackage : SqlServerApplication<SqlDatabaseDeploymentInfo>
    {
        private const string SqlPackageExe = "SqlPackage.exe";

        public SqlPackage(SqlDatabaseDeploymentInfo deploymentInfo)
            : base(deploymentInfo)
        {
        }

        protected override FileInfo GetSqlApplication()
        {
            const string SqlPackageExeDirectory = @"C:\Program Files (x86)\Microsoft SQL Server\130\DAC\bin";
            if (Directory.Exists(SqlPackageExeDirectory))
            {
                return new FileInfo(Path.Combine(SqlPackageExeDirectory, SqlPackageExe));
            }

            return new FileInfo(
                Path.Combine(@"C:\Program Files (x86)\Microsoft SQL Server\110\DAC\bin", SqlPackageExe));
        }

        protected override string GetSqlApplicationParameters()
        {
            // only publish action for now
            return string.Format(
                @"/Action:{0} /SourceFile:""{1}"" /TargetServerName:""{2}"" /TargetDatabaseName:""{3}"" /TargetUser:""{4}"" /TargetPassword:""{5}"" {6} /P:CreateNewDatabase={7} /P:DeployDatabaseInSingleUserMode={8} {9}",
                "Publish",
                DeploymentInfo.Item,
                DeploymentInfo.TargetServerName,
                DeploymentInfo.TargetDatabaseName,
                DeploymentInfo.TargetUsername,
                DeploymentInfo.TargetPassword,
                DeploymentInfo.SqlCmdVariables,
                DeploymentInfo.DoCreateNewDatabase,
                DeploymentInfo.DoDeployDatabaseInSingleUserMode,
                @"/P:DisableAndReenableDdlTriggers=True /P:IgnoreUserSettingsObjects=True");
        }
    }
}