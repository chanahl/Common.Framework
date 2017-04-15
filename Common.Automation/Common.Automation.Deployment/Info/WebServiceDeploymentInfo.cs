// <copyright file="WebServiceDeploymentInfo.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.IO;

namespace Common.Automation.Deployment.Info
{
    public class WebServiceDeploymentInfo : DeploymentInfo
    {
        public string DestinationServer { get; set; }

        public DirectoryInfo PackageLocation { get; set; }

        public FileInfo ProjectFile { get; set; }

        public string WebApplicationName { get; set; }
    }
}