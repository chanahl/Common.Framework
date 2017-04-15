// <copyright file="Constants.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

namespace Common.Automation.Bots
{
    public class Constants
    {
        #region Bot

        public const string WatchDirectory = "WatchDirectory";

        public const string WatchFile = "WatchFile";

        #endregion // Bot

        #region OutlookService

        public const string MailRecipients = "MailRecipients";

        public const string MailSubject = "MailSubject";

        #endregion // OutlookService

        #region Notifier

        public const string ShareDirectory = "ShareDirectory";

        #endregion // Notifier

        #region Default Values
        
        public const string DefaultDomain = "@td.com";

        public const string DefaultMailSubjectPrefix = "Notify";

        #endregion // Default Values
    }
}