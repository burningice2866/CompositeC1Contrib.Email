﻿using System;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text.RegularExpressions;

using Composite;
using Composite.Data;

using CompositeC1Contrib.Email.Data;
using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    public static class MailsFacade
    {
        private const string Pattern = @"(?i)\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b";
        private static readonly Regex Regex = new Regex(Pattern, RegexOptions.Compiled);

        private static readonly SmtpSection SmtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

        public delegate void MailEventHandler(object sender, MailEventEventArgs e);

        public static event MailEventHandler Queing;
        public static event MailEventHandler Queued;

        public static event MailEventHandler Sending;
        public static event MailEventHandler Sent;

        /// <summary>
        /// Validates an emailadress
        /// </summary>
        /// <param name="email">The email address to validate</param>
        /// <returns>True if validation failed, false if the email is valid</returns>
        public static bool ValidateMailAddress(string email)
        {
            if (email == null)
            {
                return true;
            }

            if (email.Length > 254)
            {
                return true;
            }

            return !Regex.IsMatch(email);
        }

        public static string[] GetMailQueueNames()
        {
            return MailQueuesFacade.GetMailQueues().Select(q => q.Name).ToArray();
        }

        public static MailQueue GetDefaultMailQueue()
        {
            var queue = MailQueuesFacade.GetMailQueues().FirstOrDefault();
            if (queue == null)
            {
                throw new InvalidOperationException("There are no queues configured, unable to process mails");
            }

            return queue;
        }

        public static IQueuedMailMessage BuildMessageAndEnqueue(object mailModel)
        {
            var message = MailModelsFacade.BuildMailMessage(mailModel);

            return EnqueueMessage(message);
        }

        public static IQueuedMailMessage EnqueueMessage(MailMessage mailMessage)
        {
            var defaultQuueue = GetDefaultMailQueue();

            return EnqueueMessage(defaultQuueue, mailMessage);
        }

        public static IQueuedMailMessage EnqueueMessage(string queueName, MailMessage mailMessage)
        {
            var queue = MailQueuesFacade.GetMailQueue(queueName);
            if (queue == null)
            {
                throw new ArgumentException(String.Format("Unknown queue name '{0}'", queueName), "queueName");
            }

            return EnqueueMessage(queue, mailMessage);
        }

        public static IQueuedMailMessage EnqueueMessage(MailQueue queue, MailMessage mailMessage)
        {
            Verify.ArgumentNotNull(queue, "queue");
            Verify.ArgumentNotNull(mailMessage, "mailMessage");

            using (var data = new DataConnection())
            {
                if (mailMessage.From == null)
                {
                    var from = queue.From;
                    if (String.IsNullOrEmpty(from))
                    {
                        from = SmtpSection.From;
                    }

                    mailMessage.From = new MailAddress(from);
                }

                var message = data.CreateNew<IQueuedMailMessage>();

                message.Id = Guid.NewGuid();
                message.TimeStamp = DateTime.UtcNow;
                message.QueueId = queue.Id;

                var templateKey = mailMessage.Headers["X-C1Contrib-Mail-TemplateKey"];
                if (!String.IsNullOrEmpty(templateKey) && data.Get<IMailTemplate>().Any(t => t.Key == templateKey))
                {
                    message.MailTemplateKey = templateKey;
                }

                if (Queing != null)
                {
                    var eventArgs = new MailEventEventArgs(message.Id, queue.Id, message.TimeStamp, message.MailTemplateKey, mailMessage);

                    Queing(null, eventArgs);
                }

                message.Subject = mailMessage.Subject;
                message.SerializedMessage = MailMessageSerializeFacade.SerializeAsBase64(mailMessage);

                data.Add(message);
                data.Log("Enqueued", String.Empty, message);

                if (Queued != null)
                {
                    var eventArgs = new MailEventEventArgs(message.Id, queue.Id, message.TimeStamp, message.MailTemplateKey, mailMessage);

                    Queued(null, eventArgs);
                }

                MailWorker.ProcessQueuesNow();

                return message;
            }
        }

        public static void EncryptMessage(MailMessage mailMessage, string password)
        {
            EncryptionHelper.EncryptMessage(mailMessage, password);
        }

        public static void SendQueuedMessage(IQueuedMailMessage message, DataConnection data, MailQueue queue)
        {
            var mailMessage = MailMessageSerializeFacade.DeserializeFromBase64(message.SerializedMessage);

            if (Sending != null)
            {
                var eventArgs = new MailEventEventArgs(message.Id, queue.Id, message.TimeStamp, message.MailTemplateKey, mailMessage);

                Sending(null, eventArgs);
            }

            mailMessage = MailMessageSerializeFacade.DeserializeFromBase64(message.SerializedMessage);

            queue.SmtpClient.Send(mailMessage);

            var sentMailMessage = data.CreateNew<ISentMailMessage>();

            sentMailMessage.Id = message.Id;
            sentMailMessage.QueueId = message.QueueId;
            sentMailMessage.MailTemplateKey = message.MailTemplateKey;
            sentMailMessage.TimeStamp = DateTime.UtcNow;
            sentMailMessage.Subject = mailMessage.Subject;

            data.Add(sentMailMessage);

            MailMessageSerializeFacade.SaveMailMessageToDisk(sentMailMessage.Id, mailMessage);

            data.Log("Sent", String.Empty, message);
            data.Delete(message);

            if (Sent != null)
            {
                var eventArgs = new MailEventEventArgs(message.Id, queue.Id, message.TimeStamp, message.MailTemplateKey, mailMessage);

                Sent(null, eventArgs);
            }
        }
    }
}
