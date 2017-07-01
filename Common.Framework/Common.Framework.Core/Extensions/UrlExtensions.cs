// <copyright file="UrlUtilities.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Common.Framework.Core.Extensions
{
    public static class UrlExtensions
    {
        public const string DefaultProtocol = @"http://";

        public static string GetProtocol(
            this string url,
            string protocol = null)
        {
            return url.Contains(@"://")
                ? new Uri(url).Scheme
                : new Uri((string.IsNullOrEmpty(protocol) ? DefaultProtocol : protocol) + url).Scheme;
        }

        public static string GetHostname(
            this string url,
            string protocol = null)
        {
            return url.Contains(@"://")
                ? new Uri(url).Host
                : new Uri((string.IsNullOrEmpty(protocol) ? DefaultProtocol : protocol) + url).Host;
        }

        public static string GetSubdomainUniqueName(
            this string url,
            string protocol = null)
        {
            return url.Contains(@"://")
                ? new Uri(url).Host.Split('.')[0]
                : new Uri((string.IsNullOrEmpty(protocol) ? DefaultProtocol : protocol) + url).Host.Split('.')[0];
        }

        public static string GetPort(
            this string url,
            string protocol = null)
        {
            return url.Contains(@"://")
                ? new Uri(url).Port.ToString(CultureInfo.InvariantCulture)
                : new Uri((string.IsNullOrEmpty(protocol) ? DefaultProtocol : protocol) + url).Port.ToString(CultureInfo.InvariantCulture);
        }

        public static string GetPath(
            this string url,
            string protocol = null)
        {
            return url.Contains(@"://")
                ? new Uri(url).LocalPath
                : new Uri((string.IsNullOrEmpty(protocol) ? DefaultProtocol : protocol) + url).LocalPath;
        }

        public static Dictionary<string, string> GetParameters(
            this string url,
            string protocol = null)
        {
            var query = url.Contains(@"://")
                ? new Uri(url).Query.Replace("?", string.Empty)
                : new Uri((string.IsNullOrEmpty(protocol) ? DefaultProtocol : protocol) + url).Query.Replace("?", string.Empty);
            var pairs = query.Split('&');
            var parameters = new Dictionary<string, string>();
            foreach (var pair in pairs)
            {
                if (!pair.Contains("="))
                {
                    continue;
                }

                var keyValuePair = pair.Split('=');
                parameters.Add(keyValuePair[0], keyValuePair[1]);
            }

            return parameters;
        }
    }
}