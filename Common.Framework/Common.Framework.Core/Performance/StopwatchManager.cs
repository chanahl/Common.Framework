// <copyright file="StopwatchManager.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Diagnostics;
using System.Threading;
using Common.Framework.Core.Patterns;

namespace Common.Framework.Core.Performance
{
    public class StopwatchManager : Singleton<StopwatchManager, Stopwatch>
    {
        public long Milliseconds { get; set; }

        public int Seconds
        {
            get { return (int)TimeSpan.FromMilliseconds(Milliseconds).TotalSeconds; }
        }

        public int Minutes
        {
            get { return (int)TimeSpan.FromMilliseconds(Milliseconds).TotalMinutes % 60; }
        }

        public int Hours
        {
            get { return (int)(TimeSpan.FromMilliseconds(Milliseconds).TotalSeconds / 1000) % 60; }
        }

        public long GetElapsedTime()
        {
            return Property.ElapsedMilliseconds;
        }

        public void WarmupAndStart()
        {
            InitializeProperty();

            // prevent JIT compiler from optimizing FKT calls
            long seed = Environment.TickCount;

            // a long warm up count.
            const int WarmUpCount = 100000000;

            // time needed to stabilize CPU cache and pipeline.
            const int WarmUpTime = 1337;

            // use second core or processor for warm-up
            Process.GetCurrentProcess().ProcessorAffinity = new IntPtr(2);

            // prevent normal processes from interrupting threads
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

            // prevent normal threads from interrupting this thread
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            // warm up
            Reset();
            Start();
            while (Property.ElapsedMilliseconds < WarmUpTime)
            {
                Warmup(seed, WarmUpCount);
            }

            Stop();

            // warm up done; start actual timing
            Reset();
            Start();
        }

        public void Reset()
        {
            Property.Reset();
        }

        public void Start()
        {
            Property.Start();
        }

        public void Stop()
        {
            Property.Stop();

            Milliseconds = GetElapsedTime();
        }

        private static void Warmup(long seed, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                // some useless bit operations
                seed ^= i ^ seed;
            }
        }
    }
}