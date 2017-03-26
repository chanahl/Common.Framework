// <copyright file="StringExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Common.Framework.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ResolveAsUnc(this string server)
        {
            return server.StartsWith(@"\\") ? server : @"\\" + server;
        }

        public static string ResolveAsUnc(
            this string server,
            string path)
        {
            return
                server.StartsWith(@"\\")
                    ? path.Contains(":")
                        ? Path.Combine(server, path.Replace(":", "$"))
                        : Path.Combine(server, path)
                    : path.Contains(":")
                        ? Path.Combine(@"\\", server, path.Replace(":", "$"))
                        : Path.Combine(@"\\", server, path);
        }

        public static string Separate(this string pascalCaseString)
        {
            var regex = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])");
            return regex.Replace(pascalCaseString, " ");
        }

        public static string ToOrdinalSuffix(this string number)
        {
            var digit = number[number.Length - 1];
            switch (digit)
            {
                case '1':
                    return "st";
                case '2':
                    return "nd";
                case '3':
                    return "rd";
                default:
                    return "th";
            }
        }

        public static string ToString(
            this DateTime dateTime,
            string format)
        {
            return dateTime.ToString(format);
        }
    }
}