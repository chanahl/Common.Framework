// <copyright file="EnvironmentType.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.ComponentModel;

namespace Common.Framework.Core.Enums
{
    public enum EnvironmentType
    {
        [Description("DEV")]
        Development = 0,

        [Description("SIT")]
        SystemIntegrationTesting = 1,

        [Description("UAT")]
        UserAcceptanceTesting = 2,

        [Description("PAT")]
        ProductionAcceptanceTesting = 4,

        [Description("PROD")]
        Production = 5
    }
}