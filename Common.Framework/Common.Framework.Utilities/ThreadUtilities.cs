// <copyright file="ThreadUtilities.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.Threading;

namespace Common.Framework.Utilities
{
    public static class ThreadUtilities
    {
        public static void Sleep(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }

        public static void Wait(
            string format,
            string message)
        {
            var count = 0;
            while (true)
            {
                if ((count % 30) == 0)
                {
                    ConsoleUtilities.WriteToConsole(format, message);
                }

                Sleep(10);
                count += 1;
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}