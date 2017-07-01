// <copyright file="Connection.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.IO;
using System.Timers;
using Common.Framework.Core;
using Common.Framework.Utilities;

namespace Common.Framework.Network.System
{
    public class Connection
    {
        private readonly object _lock = new object();

        private TimeSpan _checkInterval;

        [IODescription("ConnectionStateChanged_DisconnectedOrReconnected")]
        public event EventHandler<ConnectionStateChangedEventArgs> ConnectionStateChanged;

        public TimeSpan CheckInterval
        {
            get
            {
                return _checkInterval;
            }

            set
            {
                if (value.TotalMilliseconds < 1000)
                {
                    throw new ArgumentException("Value is too low (less than 1 second).");
                }

                _checkInterval = value;
            }
        }

        public ConnectionState ConnectionState { get; set; }

        public int InStateCounter { get; set; }

        public Timer Timer { get; set; }

        public static void ConnectionStateChangedEventHandler(
            object sender,
            ConnectionStateChangedEventArgs e)
        {
            ConsoleUtilities.WriteToConsole(
                string.Format("Connect state changed: {0}", e.ConnectionState),
                Constants.DateTimeFormatIso8601);
        }

        public double GetTimerInterval(ConnectionState newState)
        {
            switch (newState)
            {
                case ConnectionState.Connected:
                    return CheckInterval.TotalMilliseconds;
                default:
                    return Math.Min(
                        TimeSpan.FromSeconds(InStateCounter + 1).TotalMilliseconds,
                        CheckInterval.TotalMilliseconds);
            }
        }

        public void HandleErrors(
            object sender,
            ErrorEventArgs e)
        {
            SetConnectionState(ConnectionState.Disconnected);
        }

        public void ResetTimer(
            object sender,
            FileSystemEventArgs e)
        {
            Timer.Enabled = false;
            Timer.Enabled = true;
        }

        public void SetConnectionState(ConnectionState newState)
        {
            if (ConnectionState != newState)
            {
                var connectionStateChanged = false;

                lock (_lock)
                {
                    if (ConnectionState != newState)
                    {
                        InStateCounter = 0;
                        ConnectionState = newState;
                        connectionStateChanged = true;
                    }
                }

                if (connectionStateChanged)
                {
                    OnConnectionStateChanged(new ConnectionStateChangedEventArgs(newState));
                }
            }
            else
            {
                InStateCounter++;
                if (newState == ConnectionState.Connected)
                {
                    OnConnectionStateChanged(new ConnectionStateChangedEventArgs(ConnectionState.Reconnected));
                }
            }
        }

        protected virtual void OnConnectionStateChanged(ConnectionStateChangedEventArgs eventArgs)
        {
            // Copy to temp to be thread safe.
            var temp = ConnectionStateChanged;
            if (temp != null)
            {
                temp(this, eventArgs);
            }
        }

        public class ConnectionStateChangedEventArgs : EventArgs
        {
            public ConnectionStateChangedEventArgs(ConnectionState connectionState)
            {
                ConnectionState = connectionState;
            }

            public ConnectionState ConnectionState { get; private set; }
        }
    }
}