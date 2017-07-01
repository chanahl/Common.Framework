// <copyright file="Pair.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Framework.Core.Collections.Custom
{
    public class Pair<T>
    {
        public Pair(
            T first,
            T second)
        {
            First = first;
            Second = second;
        }

        public T First { get; set; }

        public T Second { get; set; }
        
        public override bool Equals(object obj)
        {
            var pair = obj as Pair<T>;
            if (pair == null)
            {
                return false;
            }

            return First.Equals(pair.First) && Second.Equals(pair.Second);
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }
    }
}