// <copyright file="Exponentiation.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Linq;
using System.Numerics;

namespace Common.Framework.Math.Algebra
{
    public static class Exponentiation
    {
        public static BigInteger Binary(
            int radix,
            BigInteger powerInBinary,
            int modulo)
        {
            var binaryIsSet = powerInBinary.ToString().Select(s => s.Equals('1')).ToArray();
            Array.Reverse(binaryIsSet);
            BigInteger radixModp = radix;
            BigInteger product = 1;

            foreach (var b in binaryIsSet)
            {
                if (modulo != 0)
                {
                    radixModp %= modulo;
                }

                if (b)
                {
                    product *= radixModp;
                }

                radixModp = radixModp * radixModp;
            }

            return (modulo != 0) ? product % modulo : product;
        }
    }
}