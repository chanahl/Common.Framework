// <copyright file="DeploymentManager.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.IO;
using Common.Automation.Deployment.Applications;
using Common.Automation.Deployment.Info;
using Common.Framework.Core.Collections.Custom;
using Common.Framework.Core.Extensions;
using Common.Framework.Core.Logging;

namespace Common.Automation.Deployment.Managers
{
    public abstract class DeploymentManager<TDeploymentInfo> where TDeploymentInfo : DeploymentInfo
    {
        protected DeploymentManager(
            List<FileInfo> deploymentEntries,
            bool isInteractive = false)
        {
            DeploymentEntries = deploymentEntries;

            IsInteractive = isInteractive;
        }

        public List<FileInfo> DeploymentEntries { get; private set; }

        public bool IsInteractive { get; private set; }

        protected PriorityQueue<TDeploymentInfo> DeploymentItems { get; set; }

        private int DeployedItems { get; set; }

        public void Run()
        {
            DeploymentItems = new PriorityQueue<TDeploymentInfo>();

            if (InitializeDeploymentItems())
            {
                DeployItems();
            }
        }

        protected bool DeployItems()
        {
            DeployedItems = 0;

            if (DeploymentItems.Count.Equals(0))
            {
                return true;
            }

            LogManager.Instance().LogInfoMessage("Total number of items to deploy are [" + DeploymentItems.Count + "].");
            foreach (var deploymentItem in DeploymentItems.Queue)
            {
                var deploymentType = DeploymentType.Unsupported;
                try
                {
                    deploymentType = (DeploymentType)deploymentItem.GetPropertyValue("DeploymentType");
                    LogManager.Instance().LogInfoMessage("Retrieved deployment type [" + deploymentType + "].");
                }
                catch (Exception exception)
                {
                    LogManager.Instance().LogErrorMessage(exception.Message);
                    LogManager.Instance().LogErrorMessage(
                        "Could not retrieve deployment type; defaulting to [" + DeploymentType.Unsupported + "].");
                }

                var deploymentStatus = false;
                switch (deploymentType)
                {
                    case DeploymentType.AsDatabase:
                        var microsoftAnalysisServicesDeployment = new MicrosoftAnalysisServicesDeployment(
                            deploymentItem.Value as SqlDatabaseDeploymentInfo);
                        deploymentStatus = microsoftAnalysisServicesDeployment.Deploy();
                        break;
                    case DeploymentType.Dacpac:
                        var sqlPackage = new SqlPackage(deploymentItem.Value as SqlDatabaseDeploymentInfo);
                        deploymentStatus = sqlPackage.Deploy();
                        break;
                    case DeploymentType.Ispac:
                        var integrationServicesDeploymentWizard = new IntegrationServicesDeploymentWizard(
                            deploymentItem.Value as SqlIspacDeploymentInfo);
                        deploymentStatus = integrationServicesDeploymentWizard.Deploy();
                        break;
                    case DeploymentType.Sql:
                        var osql = new OSql(deploymentItem.Value as SqlDatabaseDeploymentInfo);
                        deploymentStatus = osql.Deploy();
                        break;
                        ////case DeploymentType.WebService:
                        ////    var webServiceDeployer = new WebService(deploymentItem.Value as WebServiceDeploymentInfo);
                        ////    deploymentStatus = webServiceDeployer.Deploy();
                        ////    break;
                }

                if (deploymentStatus)
                {
                    DeployedItems++;
                }

                LogManager.Instance().LogInfoMessage(
                    "Deployed [" + DeployedItems + "/" + DeploymentItems.Count + "].");
            }

            if (DeployedItems.Equals(DeploymentItems.Count))
            {
                LogManager.Instance().LogInfoMessage(
                    "Successful deployment: " + "[" + DeployedItems + "/" + DeploymentItems.Count + "].");
                return true;
            }

            LogManager.Instance().LogWarningMessage(
                "Partial deployment: " + "[" + DeployedItems + "/" + DeploymentItems.Count + "].");
            return false;
        }

        protected abstract bool InitializeDeploymentItems();
    }
}