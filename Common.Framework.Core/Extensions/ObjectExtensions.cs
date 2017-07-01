// <copyright file="ObjectExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Framework.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static object GetPropertyValue(
            this object obj,
            string name)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null)
                {
                    return null;
                }

                var type = obj.GetType();
                var propertyInfo = type.GetProperty(part);
                if (propertyInfo == null)
                {
                    return null;
                }

                obj = propertyInfo.GetValue(obj, null);
            }

            return obj;
        }
    }
}