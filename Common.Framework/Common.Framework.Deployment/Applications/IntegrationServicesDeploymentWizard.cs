// <copyright file="IntegrationServicesDeploymentWizard.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.IO;
using Common.Framework.Deployment.Info;

namespace Common.Framework.Deployment.Applications
{
    public class IntegrationServicesDeploymentWizard : SqlServerApplication<SqlIspacDeploymentInfo>
    {
        private const string IsDeploymentWizardExe = "ISDeploymentWizard.exe";

        public IntegrationServicesDeploymentWizard(
            SqlIspacDeploymentInfo deploymentInfo,
            int timeoutInminutes)
            : base(deploymentInfo, timeoutInminutes)
        {
        }

        protected override FileInfo GetSqlApplication()
        {
            var sqlApplication =
                Path.Combine(@"C:\Program Files\Microsoft SQL Server\110\DTS\Binn", IsDeploymentWizardExe);
            if (File.Exists(sqlApplication))
            {
                return new FileInfo(sqlApplication);
            }

            return new FileInfo(
                Path.Combine(@"C:\Program Files (x86)\Microsoft SQL Server\110\DTS\Binn", IsDeploymentWizardExe));
        }

        protected override string GetSqlApplicationParameters()
        {
            return string.Format(
                @"/Silent /SourceType:{0} /SourcePath:""{1}"" /DestinationServer:""{2}"" /DestinationPath:""{3}""",
                DeploymentInfo.SourceType,
                DeploymentInfo.Item,
                DeploymentInfo.DestinationServer,
                DeploymentInfo.DestinationPath);
        }

        protected override bool PreExecuteOperation()
        {
            /*
            // get catalog and folder from destinationPath (e.g. /SSISDB/folder/project)
            var destinationPath = DeploymentInfo.GetType()
                .GetProperty("DestinationPath")
                .GetValue(DeploymentInfo, null)
                .ToString()
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var ssisDb = destinationPath[0].ToUpper();

            // get dataSource previously set
            var serverName =
                DeploymentInfo.GetType()
                .GetProperty("DestinationServer")
                .GetValue(DeploymentInfo, null)
                .ToString();

            var sqlConnectionForSsisDb = new SqlConnection(
                string.Format(
                    @"Server={0};Database=master;User ID={1};Password={2};",
                    serverName,
                    new SqlDatabaseDeploymentInfo().TargetUsername,
                    new SqlDatabaseDeploymentInfo().TargetPassword));
            var integrationServices = new IntegrationServices(sqlConnectionForSsisDb);

            // check if catalog is already present; create one if not
            if (integrationServices.Catalogs[ssisDb] == null)
            {
                try
                {
                    const string Password = "passw0rd";
                    new Catalog(integrationServices, ssisDb, Password).Create();
                    LogManager.Instance().LogInfoMessage(
                        "Created new catalog [" + ssisDb + "] with password [" + Password + "].");
                }
                catch (IntegrationServicesException integrationServicesException)
                {
                    LogManager.Instance().LogErrorMessage(integrationServicesException.Message);
                    return false;
                }
            }

            // check if folder is already present; create one if not
            var catalog = integrationServices.Catalogs[ssisDb];
            if (catalog == null)
            {
                LogManager.Instance().LogErrorMessage("Could not access catalog [" + ssisDb + "].");
                return false;
            }

            var folderName = destinationPath[1];
            if (catalog.Folders[folderName] == null)
            {
                // new CatalogFolder(catalog, folderName, "Created from code programmatically.").Create();
                try
                {
                    using (var sqlConnectionForFolder = new SqlConnection(
                        string.Format(@"Server={0};Initial Catalog={1};Integrated Security=SSPI;", serverName, ssisDb)))
                    {
                        sqlConnectionForFolder.Open();
                        using (var sqlCommand = new SqlCommand("[catalog].[create_folder]", sqlConnectionForFolder))
                        {
                            sqlCommand.CommandType = CommandType.StoredProcedure;
                            sqlCommand.Parameters.Add("@folder_name", SqlDbType.NVarChar, 128).Value = folderName;
                            sqlCommand.ExecuteNonQuery();
                        }

                        sqlConnectionForFolder.Close();
                    }
                }
                catch (SqlException sqlException)
                {
                    LogManager.Instance().LogErrorMessage(sqlException.Message);
                }
            }
            */

            // check if project is already deployed; drop and redeploy
            ////var projectName = destinationPath[2];
            ////var catalogFolder = catalog.Folders[folderName];
            ////if (catalogFolder != null && catalogFolder.Projects[projectName] != null)
            ////{
            ////    catalogFolder.Projects[projectName].Drop();
            ////}

            return true;
        }
    }
}