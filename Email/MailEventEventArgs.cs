using System;
using System.Net.Mail;

namespace CompositeC1Contrib.Email
{
    public class MailEventEventArgs : EventArgs
    {
        public Guid Id { get; private set; }
        public Guid QueueId { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string TemplateKey { get; private set; }
        public MailMessage MailMessage { get; private set; }

        public MailEventEventArgs(Guid id, Guid queueId, DateTime timeStamp, string templateKey, MailMessage mailMessage)
        {
            Id = id;
            QueueId = queueId;
            Timestamp = timeStamp;
            TemplateKey = templateKey;
            MailMessage = mailMessage;
        }
    }
}
