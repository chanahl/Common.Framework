// <copyright file="StringExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.ComponentModel;

namespace Common.Framework.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumeration)
        {
            var type = enumeration.GetType();

            var memberInfo = type.GetMember(enumeration.ToString());
            if (memberInfo.Length > 0)
            {
                var customAttributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (customAttributes.Length > 0)
                {
                    // Only retrieve the first description we find; others will be ignored.
                    return ((DescriptionAttribute)customAttributes[0]).Description;
                }
            }

            return enumeration.ToString();
        }

        public static string ToString(
            this DateTime dateTime,
            string format)
        {
            return dateTime.ToString(format);
        }
    }
}