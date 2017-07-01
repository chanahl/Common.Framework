// <copyright file="ConsoleUtilities.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;

namespace Common.Framework.Utilities
{
    public static class ConsoleUtilities
    {
        public static void WriteToConsole(
            string format,
            string message)
        {
            Console.WriteLine("[{0}] {1}", DateTime.Now.ToString(format), message);
        }
    }
}