// <copyright file="LogonType.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Framework.Network.Security
{
    public enum LogonType
    {
        Logon32LogonInteractive = 2,

        Logon32LogonNetwork = 3,

        Logon32LogonBatch = 4,

        Logon32LogonService = 5,

        Logon32LogonUnlock = 7,

        Logon32LogonNetworkClearText = 8, // Win2K or higher

        Logon32LogonNewCredentials = 9 // Win2K or higher
    }
}