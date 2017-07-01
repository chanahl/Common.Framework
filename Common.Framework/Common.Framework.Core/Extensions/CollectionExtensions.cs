// <copyright file="CollectionExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Common.Framework.Core.Extensions
{
    public static class CollectionExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this string csv)
        {
            // object used to convert from string to type T
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            if (csv.Length == 0 || string.IsNullOrEmpty(csv))
            {
                return null;
            }

            return csv
                .Split(',')
                .Select(n => (T)typeConverter.ConvertFromString(n.Trim()));
        }

        public static List<T> ToList<T>(this string csv)
        {
            // object used to convert from string to type T
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            if (csv.Length == 0 || string.IsNullOrEmpty(csv))
            {
                return new List<T>();
            }

            return csv
                .Split(',')
                .Select(n => (T)typeConverter.ConvertFromString(n.Trim()))
                .ToList();
        }
    }
}