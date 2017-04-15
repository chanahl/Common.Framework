// <copyright file="MicrosoftAnalysisServicesDeployment.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.IO;
using Common.Automation.Deployment.Info;

namespace Common.Automation.Deployment.Applications
{
    public class MicrosoftAnalysisServicesDeployment : SqlServerApplication<SqlDatabaseDeploymentInfo>
    {
        private const string MicrosoftAnalysisServicesDeploymentExe = "Microsoft.AnalysisServices.Deployment.exe";

        public MicrosoftAnalysisServicesDeployment(SqlDatabaseDeploymentInfo deploymentInfo) : base(deploymentInfo)
        {
        }

        protected override FileInfo GetSqlApplication()
        {
            return new FileInfo(
                Path.Combine(
                    @"C:\Program Files (x86)\Microsoft SQL Server\110\Tools\Binn\ManagementStudio",
                    MicrosoftAnalysisServicesDeploymentExe));
        }

        protected override string GetSqlApplicationParameters()
        {
            return string.Format(
                @"""{0}"" /S:""{1}""",
                DeploymentInfo.Item,
                DeploymentInfo.LogFile);
        }
    }
}