// <copyright file="Singleton.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Framework.Core.Patterns
{
    public class Singleton<TInstance, TProperty> where TInstance : new() where TProperty : new()
    {
        private static TInstance _instance;

        public TProperty Property { get; private set; }

        public static TInstance Instance()
        {
            if (_instance == null)
            {
                _instance = new TInstance();
            }

            return _instance;
        }

        public void InitializeProperty()
        {
            Property = new TProperty();
        }
    }
}