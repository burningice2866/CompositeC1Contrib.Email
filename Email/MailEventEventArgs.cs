using System;
using System.Net.Mail;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    public class MailEventEventArgs : EventArgs
    {
        private readonly IMailMessage _iMailMessage;

        public MailMessage MailMessage { get; private set; }

        public Guid Id
        {
            get { return _iMailMessage.Id; }
        }

        public Guid QueueId
        {
            get { return _iMailMessage.QueueId; }
        }

        public DateTime Timestamp
        {
            get { return _iMailMessage.TimeStamp; }
        }

        public string TemplateKey
        {
            get { return _iMailMessage.MailTemplateKey; }
        }

        public MailEventEventArgs(IMailMessage iMailMessage, MailMessage mailMessage)
        {
            _iMailMessage = iMailMessage;
            MailMessage = mailMessage;
        }
    }
}
