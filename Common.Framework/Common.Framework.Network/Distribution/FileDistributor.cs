// <copyright file="FileDistributor.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.IO;
using Common.Framework.Core.Extensions;
using Common.Framework.Core.Logging;

namespace Common.Framework.Network.Distribution
{
    public abstract class FileDistributor : Distributor
    {
        protected Dictionary<FileInfo, FileInfo> DistributionInfo { get; set; }

        protected FileTransfer FileTransfer { get; set; }

        public override void Initialize()
        {
            DistributionInfo = new Dictionary<FileInfo, FileInfo>();

            FileTransfer = new FileTransfer(true);

            GetDistributionInfo();
        }

        public override void Distribute()
        {
            FileTransfer.DistributionInfo = DistributionInfo;
            FileTransfer.Distribute();
        }

        protected void AddDistributionInfo(
            string source,
            string destination)
        {
            var files = new List<string>();
            var info = source.IsDirectoryOrFile();
            if (info == null)
            {
                return;
            }

            var directoryInfo = info as DirectoryInfo;
            if (directoryInfo != null)
            {
                files = directoryInfo.FullName.FindAllFiles();
            }

            var fileInfo = info as FileInfo;
            if (fileInfo != null)
            {
                files.Add(fileInfo.FullName);
            }

            foreach (var file in files)
            {
                if (!file.DirectoryOrFileExists())
                {
                    LogManager.Instance().LogWarningMessage("Skipping over [" + file + "] as it no longer exists.");
                    continue;
                }

                var directoryName = Path.GetDirectoryName(file);
                if (directoryName == null)
                {
                    continue;
                }

                var destinationFile = file.Replace(source, destination);
                try
                {
                    DistributionInfo.Add(new FileInfo(file), new FileInfo(destinationFile));
                    if (AppConfig.AppConfigCore.IsDebugMode)
                    {
                        LogManager.Instance().LogDebugMessage(
                            "Added file [" + file + "] for distribution to [" + destinationFile + "].");
                    }
                }
                catch (Exception exception)
                {
                    LogManager.Instance().LogWarningMessage(
                        "Could not add file [" + file + "].  Exception: " + exception);
                }
            }
        }

        protected abstract void GetDistributionInfo();
    }
}