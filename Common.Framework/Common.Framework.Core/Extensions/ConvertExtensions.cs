// <copyright file="ConvertExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;

namespace Common.Framework.Core.Extensions
{
    public static class ConvertExtensions
    {
        public static TEnum ToEnum<TEnum>(this string enumStringValue) where TEnum : struct
        {
            TEnum enumValue;
            if (!Enum.TryParse(enumStringValue, true, out enumValue))
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not convert [{0}] to enumerated constant of type [{1}].",
                        enumStringValue,
                        typeof(TEnum).Name));
            }

            if (!Enum.IsDefined(typeof(TEnum), enumValue))
            {
                throw new ArgumentException(
                    string.Format(
                        "The value of [{0}] was not defined in the specified enumeration of type [{1}].",
                        enumStringValue,
                        typeof(TEnum).Name));
            }

            return enumValue;
        }
    }
}