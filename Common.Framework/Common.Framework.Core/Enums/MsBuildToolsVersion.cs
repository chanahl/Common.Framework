// <copyright file="MsBuildToolsVersion.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.ComponentModel;

namespace Common.Framework.Core.Enums
{
    public enum MsBuildToolsVersion
    {
        [Description("2.0")]
        Two,

        [Description("3.5")]
        ThreeFive,

        [Description("4.0")]
        Four,

        [Description("12.0")]
        Twelve,

        [Description("14.0")]
        Fourteen
    }
}