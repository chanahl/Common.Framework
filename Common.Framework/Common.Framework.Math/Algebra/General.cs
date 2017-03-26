// <copyright file="General.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.Collections.Generic;
using System.Numerics;
using Common.Framework.Core.AppConfig;
using Common.Framework.Core.Collections.Custom;
using Common.Framework.Core.Logging;

namespace Common.Framework.Math.Algebra
{
    public static class General
    {
        public static BigInteger CalculateFactorial(this int number)
        {
            if (number == 0)
            {
                return 1;
            }

            return number * CalculateFactorial(number - 1);
        }

        public static BigInteger CalculateFactorial(this long number)
        {
            if (number == 0)
            {
                return 1;
            }

            return number * CalculateFactorial(number - 1);
        }

        public static int CalculateGreatestCommonDivisor(this Pair<int> pair)
        {
            while (true)
            {
                if (pair.First >= 0 && pair.Second >= 0)
                {
                    if (pair.Second == 0)
                    {
                        return pair.First;
                    }

                    var temporary = pair.First;
                    pair.First = pair.Second;
                    pair.Second = temporary % pair.Second;
                    continue;
                }

                return 0;
            }
        }

        public static long CalculateLargestPrimeFactor(this long number)
        {
            var remainder = number;
            long largestPrimeFactor = 0;

            var counter = 2;
            while (counter * counter < remainder)
            {
                if (remainder % counter == 0)
                {
                    remainder /= counter;
                    largestPrimeFactor = counter;
                }
                else
                {
                    counter++;
                }
            }

            if (remainder > largestPrimeFactor)
            {
                largestPrimeFactor = remainder;
            }

            return largestPrimeFactor;
        }

        public static List<int> CalculateMultiples(this Pair<int> pair, bool isExclusive)
        {
            var multiples = new List<int>();

            for (var number = 1; isExclusive ? number < pair.Second : number <= pair.Second; number++)
            {
                if (number % pair.First == 0)
                {
                    multiples.Add(number);
                }
            }

            return multiples;
        }

        public static List<int> CalculateProperDivisors(this int number)
        {
            var properDivisors = new List<int> { 1 };
            var limit = System.Math.Floor(System.Math.Sqrt(number)) + 1;

            for (var i = 2; i < limit; i++)
            {
                if (number % i != 0)
                {
                    continue;
                }

                properDivisors.Add(i);
                if (number / i != i)
                {
                    properDivisors.Add(number / i);
                }
            }

            if (AppConfigManager.Instance().Property.AppConfigCore.IsDebugMode)
            {
                properDivisors.Sort();
            }

            return properDivisors;
        }

        public static int CalculateRemainder(this int parity)
        {
            switch (parity)
            {
                case 2:
                    return 0;
                case 1:
                    return 1;
                default:
                    return -1;
            }
        }
    }
}