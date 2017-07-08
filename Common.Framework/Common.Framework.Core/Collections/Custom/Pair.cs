// <copyright file="Pair.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;

namespace Common.Framework.Core.Collections.Custom
{
    public class Pair<T> : IEquatable<Pair<T>>
    {
        public Pair(
            T first,
            T second)
        {
            First = first;
            Second = second;
        }

        public T First { get; }

        public T Second { get; }

        public bool Equals(Pair<T> pair)
        {
            if (pair == null)
            {
                return false;
            }

            return First.Equals(pair.First) && Second.Equals(pair.Second);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Pair<T>);
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }
    }
}