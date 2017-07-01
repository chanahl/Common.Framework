// <copyright file="NetworkFileSystemWatcher.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.IO;
using System.Timers;

namespace Common.Framework.Network.System
{
    public class NetworkFileSystemWatcher : FileSystemWatcher
    {
        private const int DefaultCheckConnectionInSeconds = 1800;

        public NetworkFileSystemWatcher()
        {
            Initialize();
        }

        public NetworkFileSystemWatcher(string path)
            : base(path)
        {
            Initialize();
        }

        public NetworkFileSystemWatcher(
            string path,
            string filter)
            : base(path, filter)
        {
            Initialize();
        }

        public NetworkFileSystemWatcher(TimeSpan connectionTimeSpan)
        {
            Initialize(connectionTimeSpan);
        }

        public NetworkFileSystemWatcher(
            TimeSpan connectionTimeSpan,
            string path)
            : base(path)
        {
            Initialize(connectionTimeSpan);
        }

        public NetworkFileSystemWatcher(
            TimeSpan connectionTimeSpan,
            string path,
            string filter)
            : base(path, filter)
        {
            Initialize(connectionTimeSpan);
        }

        public Connection Connection { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (Connection.Timer != null)
            {
                Error -= Connection.HandleErrors;

                Changed -= Connection.ResetTimer;
                Created -= Connection.ResetTimer;
                ////Deleted -= Connection.ResetTimer;
                ////Renamed -= Connection.ResetTimer;

                Connection.Timer.Enabled = false;
                Connection.Timer.Elapsed -= ConnectionTimerElapsedCheck;
                Connection.Timer = null;
            }

            base.Dispose(disposing);
        }

        private void Initialize()
        {
            Initialize(TimeSpan.FromSeconds(DefaultCheckConnectionInSeconds));
        }

        private void Initialize(TimeSpan checkConnectionInterval)
        {
            Connection = new Connection
            {
                CheckInterval = checkConnectionInterval,
                ConnectionState = ConnectionState.Connected
            };

            Error += Connection.HandleErrors;

            Changed += Connection.ResetTimer;
            Created += Connection.ResetTimer;
            ////Deleted += Connection.ResetTimer;
            ////Renamed += Connection.ResetTimer;

            Connection.Timer = new Timer(Connection.GetTimerInterval(Connection.ConnectionState));
            Connection.Timer.Elapsed += ConnectionTimerElapsedCheck;
            Connection.Timer.Enabled = true;
        }

        private void ConnectionTimerElapsedCheck(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            CheckConnection();
        }

        private void CheckConnection()
        {
            if (!EnableRaisingEvents && Connection.ConnectionState != ConnectionState.Disconnected)
            {
                return;
            }

            Connection.Timer.Enabled = false;
            try
            {
                EnableRaisingEvents = false;
                EnableRaisingEvents = true;

                Connection.SetConnectionState(ConnectionState.Connected);
            }
            catch
            {
                Connection.SetConnectionState(ConnectionState.Disconnected);
            }

            Connection.Timer.Interval = Connection.GetTimerInterval(Connection.ConnectionState);
            Connection.Timer.Enabled = true;
        }
    }
}