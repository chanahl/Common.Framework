// <copyright file="PriorityQueue.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System.Collections.Generic;

namespace Common.Framework.Core.Collections.Custom
{
    public class PriorityQueue<T>
    {
        private readonly SortedList<Pair<int>, T> _queue;

        public PriorityQueue()
        {
            _queue = new SortedList<Pair<int>, T>(new PairComparer<int>());
        }

        public int Count { get; private set; }

        public void Enqueue(T item, int priority)
        {
            _queue.Add(new Pair<int>(priority, Count), item);
            Count++;
        }

        public T Dequeue()
        {
            var item = _queue[_queue.Keys[0]];
            _queue.RemoveAt(0);
            return item;
        }
    }
}