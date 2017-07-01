// <copyright file="FileTransfer.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Framework.Core.Extensions;
using Common.Framework.Core.Logging;

namespace Common.Framework.Network.Distribution
{
    public class FileTransfer
    {
        public FileTransfer(bool isCleanTransfer)
        {
            IsCleanTransfer = isCleanTransfer;
        }

        public bool IsCleanTransfer { get; set; }

        public string DestinationDirectory { get; set; }

        public string DestinationDomain { get; set; }

        public Dictionary<FileInfo, FileInfo> DistributionInfo { get; set; }

        public void Distribute()
        {
            if (!string.IsNullOrEmpty(DestinationDirectory) && IsCleanTransfer)
            {
                DeleteDestinationDirectory();
            }

            foreach (var file in DistributionInfo.Select((value, index) => new { index, value }))
            {
                var source = file.value.Key.FullName;
                var destination = file.value.Value.FullName;

                var arrayLength = (int)Math.Pow(2, 19);
                var dataArray = new byte[arrayLength];

                using (var fileStreamRead =
                    new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.Read, arrayLength))
                {
                    using (var binaryReader = new BinaryReader(fileStreamRead))
                    {
                        destination.EnsureDirectory();
                        using (var fileStreamWrite =
                            new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None, arrayLength))
                        {
                            using (var binaryWriter = new BinaryWriter(fileStreamWrite))
                            {
                                for (;;)
                                {
                                    var read = binaryReader.Read(dataArray, 0, arrayLength);
                                    if (read.Equals(0))
                                    {
                                        break;
                                    }

                                    binaryWriter.Write(dataArray, 0, read);
                                }
                            }
                        }

                        var sourceFileInfo = new FileInfo(source);
                        var destinationFileInfo = new FileInfo(destination)
                        {
                            CreationTime = sourceFileInfo.CreationTime,
                            LastAccessTime = sourceFileInfo.LastAccessTime,
                            LastWriteTime = sourceFileInfo.LastWriteTime
                        };
                        destinationFileInfo.Refresh();

                        LogManager.Instance().LogInfoMessage("Copied file [" + file.index + "] [" + destination + "].");
                    }
                }
            }

            LogManager.Instance().LogInfoMessage("Distribution of [" + DestinationDirectory + "] complete.");
        }

        private void DeleteDestinationDirectory()
        {
            LogManager.Instance().LogInfoMessage("Deleting destination directory [" + DestinationDirectory + "].");
            DestinationDirectory.DeleteDirectory(true);
            LogManager.Instance().LogInfoMessage("Deleted destination directory [" + DestinationDirectory + "].");
        }
    }
}