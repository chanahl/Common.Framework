// <copyright file="SystemUtilities.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.Management;

namespace Common.Framework.Utilities.Utilities
{
    public static class SystemUtilities
    {
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