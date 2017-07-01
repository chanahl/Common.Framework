// <copyright file="IntegerExtensions.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Numerics;

namespace Common.Framework.Core.Extensions
{
    public static class IntegerExtensions
    {
        public static BigInteger ToBinary(this int number)
        {
            return BigInteger.Parse(Convert.ToString(number, 2));
        }

        public static int ToInteger(this string parity)
        {
            switch (parity.ToLower())
            {
                case "even":
                    return 2;
                case "odd":
                    return 1;
                default:
                    return 0;
            }
        }
    }
}