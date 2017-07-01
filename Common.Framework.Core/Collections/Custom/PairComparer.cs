// <copyright file="PairComparer.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Collections.Generic;

namespace Common.Framework.Core.Collections.Custom
{
    public class PairComparer<T> : IComparer<Pair<T>> where T : IComparable
    {
        public int Compare(
            Pair<T> x,
            Pair<T> y)
        {
            if (x.First.CompareTo(y.First) < 0)
            {
                return -1;
            }

            if (x.First.CompareTo(y.First) > 0)
            {
                return 1;
            }

            return x.Second.CompareTo(y.Second);
        }
    }
}