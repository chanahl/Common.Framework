// <copyright file="Bot.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.IO;
using Common.Framework.Core.AppConfig;
using Common.Framework.Core.Logging;
using Common.Framework.Network.System;
using Common.Framework.Utilities.Utilities;

namespace Common.Automation.Bots.Bots
{
    public abstract class Bot
    {
        public static readonly AppConfigCore AppConfigCore = AppConfigManager.Instance().Property.AppConfigCore;

        public static readonly AppConfigParameters AppConfigParameters =
            AppConfigManager.Instance().Property.AppConfigParameters;

        protected Bot()
        {
            Name = GetType().Name;

            WatchDirectory = AppConfigParameters.Keys[Name].Collection.ContainsKey(Constants.WatchDirectory)
                ? new DirectoryInfo(AppConfigParameters.Keys[Name].Collection[Constants.WatchDirectory])
                : null;

            if (AppConfigCore.IsDebugMode && WatchDirectory != null)
            {
                var path = WatchDirectory.FullName;
                if (path.StartsWith(@"R:\Drops"))
                {
                    WatchDirectory = new DirectoryInfo(path.Replace("R:", "D:"));
                }
            }

            WatchFile = AppConfigParameters.Keys[Name].Collection.ContainsKey(Constants.WatchFile)
                ? AppConfigParameters.Keys[Name].Collection[Constants.WatchFile]
                : string.Empty;
        }

        public string Name { get; set; }

        public DirectoryInfo WatchDirectory { get; set; }

        public string WatchFile { get; set; }

        public FileInfo DetectedFile { get; set; }

        public void Run()
        {
            if (WatchDirectory != null && Directory.Exists(WatchDirectory.FullName))
            {
                var message = "Watching [" + WatchDirectory + "] for [" + WatchFile + "].";
                ConsoleUtilities.WriteToConsole(message, Framework.Core.Constants.DateTimeFormatIso8601);
                LogManager.Instance().LogDebugMessage(message);
            }
            else
            {
                return;
            }

            var networkFileSystemWatcher = new NetworkFileSystemWatcher(new TimeSpan(0, 10, 0), WatchDirectory.FullName)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.FileName
            };
            networkFileSystemWatcher.Changed += WatcherVerification;
            networkFileSystemWatcher.Created += WatcherVerification;
            networkFileSystemWatcher.Connection.ConnectionStateChanged += WatcherConnectionStateChanged;
        }

        protected abstract void WatcherActivity();

        protected virtual object WatcherInstructions()
        {
            return null;
        }

        protected void WatcherConnectionStateChanged(
            object sender,
            Connection.ConnectionStateChangedEventArgs connectionStateChangedEventArgs)
        {
            var message = string.Format("Connection state changed: {0}", connectionStateChangedEventArgs.ConnectionState);
            ConsoleUtilities.WriteToConsole(message, Framework.Core.Constants.DateTimeFormatIso8601);
            LogManager.Instance().LogDebugMessage(message);
        }

        protected void WatcherVerification(
            object sender,
            FileSystemEventArgs file)
        {
            DetectedFile = null;
            var detectedFileDirectory = Path.GetDirectoryName(file.FullPath);
            if (detectedFileDirectory == null)
            {
                return;
            }

            var detectedFile = Path.Combine(detectedFileDirectory, WatchFile);
            if (!file.FullPath.Equals(detectedFile, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var message = "Detected file [" + file.Name + "].";
            try
            {
                ConsoleUtilities.WriteToConsole(message, Framework.Core.Constants.DateTimeFormatIso8601);
                LogManager.Instance().LogInfoMessage(message);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }

            ThreadUtilities.Sleep(5);
            DetectedFile = new FileInfo(detectedFile);
            WatcherActivity();
        }
    }
}