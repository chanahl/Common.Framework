// <copyright file="Distributor.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using Common.Framework.Core.AppConfig;

namespace Common.Framework.Network.Distribution
{
    public abstract class Distributor
    {
        protected static readonly AppConfig AppConfig =
            AppConfigManager.Instance().Property;

        protected static readonly AppConfigParameters AppConfigParameters =
            AppConfigManager.Instance().Property.AppConfigParameters;

        protected Distributor()
        {
            Environment = AppConfig.AppConfigCore.RunType;
            Name = GetType().Name;
        }

        public string Environment { get; set; }

        public string Name { get; set; }

        public abstract void Distribute();

        public abstract void Initialize();

        public virtual void Run()
        {
            Initialize();
            Distribute();
        }
    }
}