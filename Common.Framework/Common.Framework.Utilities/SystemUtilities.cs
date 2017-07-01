// <copyright file="SystemUtilities.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.IO;
using System.Management;

namespace Common.Framework.Utilities
{
    public static class SystemUtilities
    {
        public static string GetDrive(
            DriveType driveType,
            string driveLetter)
        {
            var drives = DriveInfo.GetDrives();
            foreach (var drive in drives)
            {
                var driveName = drive.Name.Substring(0, 1);
                switch (driveType)
                {
                    case DriveType.CDRom:
                    case DriveType.Fixed:
                    case DriveType.Network:
                    case DriveType.NoRootDirectory:
                    case DriveType.Ram:
                    case DriveType.Removable:
                    case DriveType.Unknown:
                        if (driveLetter.Equals(driveName))
                        {
                            return drive.Name;
                        }

                        break;
                }
            }

            return string.Empty;
        }

        public static int GetCores()
        {
            var count = 0;
            foreach (var item in new ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                count += int.Parse(item["NumberOfCores"].ToString());
            }

            return count;
        }

        public static int GetLogicalProcessors()
        {
            var count = 0;
            foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                count += int.Parse(item["NumberOfLogicalProcessors"].ToString());
            }

            return count;
        }

        public static int GetPhysicalProcessors()
        {
            var count = 0;
            foreach (var item in new ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                count += int.Parse(item["NumberOfProcessors"].ToString());
            }

            return count;
        }
    }
}