// <copyright file="DateTimeExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Globalization;

namespace Common.Framework.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime NewDateTime(
            this string date,
            string format)
        {
            if (date.Length == 0)
            {
                return new DateTime();
            }

            var newDateTime = DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
            return newDateTime;
        }
    }
}