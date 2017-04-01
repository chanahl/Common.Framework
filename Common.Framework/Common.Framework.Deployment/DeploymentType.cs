// <copyright file="DeploymentType.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Framework.Deployment
{
    public enum DeploymentType
    {
        Unsupported,

        AsDatabase,

        Dacpac,

        FileSystemObject,

        Ispac,

        SoaService,

        Sql,

        WebService
    }
}