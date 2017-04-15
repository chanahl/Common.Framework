// <copyright file="OSql.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.IO;
using Common.Automation.Deployment.Info;

namespace Common.Automation.Deployment.Applications
{
    public class OSql : SqlServerApplication<SqlDatabaseDeploymentInfo>
    {
        private const string OSqlExe = "OSQL.exe";

        public OSql(SqlDatabaseDeploymentInfo deploymentInfo) : base(deploymentInfo)
        {
        }

        protected override FileInfo GetSqlApplication()
        {
            return new FileInfo(
                Path.Combine(@"C:\Program Files\Microsoft SQL Server\110\Tools\Binn", OSqlExe));
        }

        protected override string GetSqlApplicationParameters()
        {
            var tdn = DeploymentInfo.TargetDatabaseName;
            return string.Format(
                @"-i ""{0}"" -S ""{1}"" {2} -U ""{3}"" -P ""{4}""",
                DeploymentInfo.Item,
                DeploymentInfo.TargetServerName,
                !string.IsNullOrEmpty(tdn) ? @"-d """ + tdn + @"""" : string.Empty,
                DeploymentInfo.TargetUsername,
                DeploymentInfo.TargetPassword);
        }
    }
}