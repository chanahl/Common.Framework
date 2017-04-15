// <copyright file="Notify.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.IO;
using Common.Framework.Core.Extensions;
using Common.Framework.Core.Logging;
using Common.Framework.Utilities.Utilities;

namespace Common.Automation.Bots.Bots
{
    public abstract class Notify : OutlookService
    {
        protected Notify()
        {
            ShareDirectory = AppConfigParameters.Keys[Name].Collection.ContainsKey(Constants.ShareDirectory)
                ? !string.IsNullOrEmpty(AppConfigParameters.Keys[Name].Collection[Constants.ShareDirectory])
                    ? new DirectoryInfo(AppConfigParameters.Keys[Name].Collection[Constants.ShareDirectory])
                    : null
                : null;

            ShareSubDirectory = null;
        }

        public DirectoryInfo ShareDirectory { get; set; }

        public DirectoryInfo ShareSubDirectory { get; set; }

        protected abstract void Initialize();

        protected virtual void CopyContentsToShareDirectory()
        {
            try
            {
                ThreadUtilities.Sleep(5);
                Path.GetDirectoryName(DetectedFile.FullName).CopyDirectoryTo(ShareSubDirectory.FullName);
                ShareDirectory.FullName.RemoveDirectories(Name, 10);
            }
            catch (Exception exception)
            {
                ConsoleUtilities.WriteToConsole(exception.Message, Framework.Core.Constants.DateTimeFormatIso8601);
                LogManager.Instance().LogErrorMessage(exception.Message);
            }
        }

        protected override void WatcherActivity()
        {
            Initialize();

            HtmlBody += WatcherInstructions();

            Send();

            Dispose();
        }

        protected virtual void Dispose()
        {
            HtmlBody = string.Empty;
        }
    }
}