using System;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    public static class Logger
    {
        public static void Log(this DataConnection data, string eventName, string eventContent, IMailMessage message)
        {
            var logItem = data.CreateNew<IMailMessageLogItem>();

            logItem.Id = Guid.NewGuid();
            logItem.MailMessageId = message.Id;
            logItem.Timestamp = DateTime.UtcNow;
            logItem.EventName = eventName;
            logItem.EventData = eventContent;

            data.Add(logItem);
        }
    }
}
