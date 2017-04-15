// <copyright file="OutlookService.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.IO;
using Common.Framework.Core.Logging;
using Common.Framework.Utilities.Utilities;
using Microsoft.Office.Interop.Outlook;

namespace Common.Automation.Bots.Bots
{
    public abstract class OutlookService : Bot
    {
        protected OutlookService()
        {
            Attachment = null;

            HtmlBody = string.Empty;

            Application = new Application();

            Items = Application
                .GetNamespace("MAPI")
                .GetDefaultFolder(OlDefaultFolders.olFolderInbox)
                .Items;

            Items.ItemAdd += Items_ItemAdd;

            MailDomain = Constants.DefaultDomain;

            MailRecipients = AppConfigParameters.Keys[Name].Collection.ContainsKey(Constants.MailRecipients)
                ? AppConfigParameters.Keys[Name].Collection[Constants.MailRecipients]
                : string.Empty;

            MailSubject = AppConfigParameters.Keys[Name].Collection.ContainsKey(Constants.MailSubject)
                ? AppConfigParameters.Keys[Name].Collection[Constants.MailSubject]
                : string.Empty;

            MailSubjectPrefix = Constants.DefaultMailSubjectPrefix;
        }

        protected FileInfo Attachment { get; set; }

        protected string HtmlBody { get; set; }

        private Application Application { get; set; }

        private Items Items { get; set; }

        private string MailDomain { get; set; }

        private string MailRecipients { get; set; }

        private string MailSubject { get; set; }

        private string MailSubjectPrefix { get; set; }

        public void Reply(dynamic item)
        {
            var response = item.ReplyAll();
            response.HTMLBody =
                "<font face='courier new' size='3'>" +
                "<b>--------------------------------------------------------------------------------</b>" +
                "<br>" +
                "<b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; > This is an automated response.</b>" +
                "<br>" +
                "<b>--------------------------------------------------------------------------------</b>" +
                "</font>" +
                "<br><br>" +
                HtmlBody +
                response.HTMLBody;
            response.Send();
        }

        public void Send()
        {
            try
            {
                var mailItem = Application.CreateItem(OlItemType.olMailItem);

                mailItem.Subject = string.Format("[{0}] {1}", MailSubjectPrefix, MailSubject);
                mailItem.HTMLBody = HtmlBody;

                if (Attachment != null)
                {
                    var attachment = Attachment.FullName;
                    if (!Path.IsPathRooted(attachment))
                    {
                        attachment = Path.Combine(Directory.GetCurrentDirectory(), attachment);
                    }

                    mailItem.Attachments.Add(attachment, OlAttachmentType.olByValue, Type.Missing, Type.Missing);
                }

                if (MailRecipients.Length.Equals(0) || string.IsNullOrEmpty(MailRecipients))
                {
                    return;
                }

                var mailItemRecipients = mailItem.Recipients;
                var recipients = MailRecipients.Split(',');
                foreach (var recipient in recipients)
                {
                    var mailItemRecipient =
                        recipient.EndsWith("Notify")
                            ? mailItemRecipients.Add(recipient)
                            : mailItemRecipients.Add(recipient + MailDomain);
                    mailItemRecipient.Resolve();
                }

                mailItem.Send();
                var message = "Email sent in response to [" + WatchFile + "].";
                ConsoleUtilities.WriteToConsole(message, Framework.Core.Constants.DateTimeFormatIso8601);
                LogManager.Instance().LogInfoMessage(message);
            }
            catch (SystemException systemException)
            {
                var systemExceptionMessage = "Exception: " + systemException.Message;
                ConsoleUtilities.WriteToConsole(systemExceptionMessage, Framework.Core.Constants.DateTimeFormatIso8601);
                LogManager.Instance().LogErrorMessage(systemExceptionMessage);
            }
        }

        protected virtual void Items_ItemAdd(object item)
        {
        }
    }
}