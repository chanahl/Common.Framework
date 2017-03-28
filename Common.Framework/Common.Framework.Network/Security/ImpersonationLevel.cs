// <copyright file="ImpersonationLevel.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Framework.Network.Security
{
    public enum ImpersonationLevel
    {
        SecurityAnonymous = 0,

        SecurityIdentification = 1,

        SecurityImpersonation = 2,

        SecurityDelegation = 3
    }
}