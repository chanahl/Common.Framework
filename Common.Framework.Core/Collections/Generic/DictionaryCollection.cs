// <copyright file="DictionaryCollection.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common.Framework.Core.Enums;
using Common.Framework.Core.Extensions;

namespace Common.Framework.Core.Collections.Generic
{
    public class DictionaryCollection
    {
        public DictionaryCollection()
        {
            Collection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public Dictionary<string, string> Collection { get; set; }

        public string this[string key]
        {
            // return stored value for given key
            get { return GetKey(key); }

            // assign to this element the value
            set { SetValue(key, value); }
        }

        public static IEnumerable<string> CleanValues(
            string values,
            DelimiterType delimiterType)
        {
            var delimiter = delimiterType.GetDescription();

            var cleanValues = values
                .TrimStart().Trim().TrimEnd()
                .Split(new[] { delimiter }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            for (var i = 0; i < cleanValues.Count; i++)
            {
                cleanValues[i] = cleanValues[i]
                    .Replace(delimiter.ToString(CultureInfo.InvariantCulture), string.Empty)
                    .Replace("\r\n", string.Empty)
                    .Replace("\n", string.Empty)
                    .Replace("\r", string.Empty)
                    .Trim();
            }

            return cleanValues;
        }

        private string GetKey(string key)
        {
            if (!Collection.ContainsKey(key))
            {
                return string.Empty;
            }

            var keyName = Collection[key];
            return keyName;
        }

        private void SetValue(string key, string value)
        {
            Collection[key] = value;
        }
    }
}