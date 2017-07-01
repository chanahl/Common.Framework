// <copyright file="LogonProvider.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Framework.Network.Security
{
    public enum LogonProvider
    {
        Logon32ProviderDefault = 0,

        Logon32ProviderWinNt35 = 1,

        Logon32ProviderWinNt40 = 2,

        Logon32ProviderWinNt50 = 3
    }
}