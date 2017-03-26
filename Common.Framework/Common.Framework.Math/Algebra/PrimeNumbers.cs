// <copyright file="PrimeNumbers.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Framework.Math.Algebra
{
    public static class PrimeNumbers
    {
        public delegate int[] PrimeNumberGenerator(int limit);

        public static int[] GetPrimes(int limit, PrimeNumberGenerator primeNumberGenerator)
        {
            var primes = primeNumberGenerator(limit);
            return primes;
        }

        public static int[] PrimesBySieveOfAtkin(int limit)
        {
            // Initialize the Sieve.
            var sieve = new bool[limit + 1];

            // Put in candidate primes: integers which have an odd number of representations by certain quadratic forms.
            var sqrt = (int)System.Math.Sqrt(limit);
            int x;
            int n;
            for (x = 1; x <= sqrt; x++)
            {
                int y;
                for (y = 1; y <= sqrt; y++)
                {
                    // Sieve of Atkin
                    n = (4 * x * x) + (y * y);
                    if (n <= limit && (n % 12 == 1 || n % 12 == 5))
                    {
                        sieve[n] ^= true;
                    }

                    n = (3 * x * x) + (y * y);
                    if (n <= limit && n % 12 == 7)
                    {
                        sieve[n] ^= true;
                    }

                    n = (3 * x * x) - (y * y);
                    if (x > y && n <= limit && n % 12 == 11)
                    {
                        sieve[n] ^= true;
                    }
                }
            }

            // Eliminate composites by sieving.
            for (n = 5; n <= sqrt; n++)
            {
                if (!sieve[n])
                {
                    continue;
                }

                // n is prime, omit multiples of its square; this is sufficient because composites which managed to get on the list cannot be square-free.
                for (var k = n * n; k <= limit; k += n * n)
                {
                    sieve[k] = false;
                }
            }

            // Initialize list of starting primes.
            var index = 2;
            var list = new int[(limit / 2) + 1];
            list[0] = 2;
            list[1] = 3;
            for (n = 5; n < limit; n++)
            {
                if (!sieve[n])
                {
                    continue;
                }

                list[index] = n;
                index++;
            }

            // Retrieve list of primes.
            var primes = new int[index];
            Array.Copy(list, 0, primes, 0, index);

            return primes;
        }

        public static int[] PrimesByTrialDivision(int limt)
        {
            var primes = new List<int> { 2 };

            for (var i = 3; i <= limt; i++)
            {
                var j = 0;
                var isPrime = true;

                // Check all odd numbers to see if they are already divisible by an already found prime[j], starting with 2.  Need only check up to square root of the potential prime.
                while (primes[j] * primes[j] <= i)
                {
                    // Even numbers cannot be prime.
                    if (i % primes[j] == 0)
                    {
                        isPrime = false;
                        break;
                    }

                    // Check next potential prime.
                    j++;
                }

                if (isPrime)
                {
                    primes.Add(i);
                }
            }

            return primes.ToArray<int>();
        }

        public static int SieveLimit(double n)
        {
            return (int)(
                (n * System.Math.Log(n)) +
                (n * System.Math.Log(System.Math.Log(n))));
        }
    }
}