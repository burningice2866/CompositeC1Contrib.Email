using System;
using System.Net.Mail;

using CompositeC1Contrib.Email.Web.UI;

namespace CompositeC1Contrib.Email.SignalR
{
    public class MailMessageModel
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string QueueId { get; set; }
        public string TemplateKey { get; set; }
        public string TimeStamp { get; set; }

        public MailMessageModel(Guid id, Guid queueId, DateTime timeStamp, string templateKey, MailMessage message)
        {
            var logItem = new MailLogItem(id, timeStamp, templateKey, message.Subject);
            
            Id = id.ToString();
            QueueId = queueId.ToString();
            TemplateKey = templateKey ?? "No template";
            Subject = message.Subject;
            TimeStamp = logItem.FormatTimeStamp();
        }
    }
}