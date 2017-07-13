// <copyright file="Singleton.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Framework.Core.Patterns
{
    public class Singleton<TInstance, TProperty> where TInstance : new() where TProperty : new()
    {
        public TProperty Property { get; private set; }

        private static TInstance TheInstance { get; set; }

        public static TInstance Instance()
        {
            if (TheInstance == null)
            {
                TheInstance = new TInstance();
            }

            return TheInstance;
        }

        public void InitializeProperty()
        {
            Property = new TProperty();
        }
    }
}