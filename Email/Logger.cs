using System;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    public static class Logger
    {
        public static void LogBasicEvent(this DataConnection data, string @event, IMailMessage message)
        {
            var logItem = data.CreateNew<IEventBasic>();

            logItem.Id = Guid.NewGuid();
            logItem.MailMessageId = message.Id;
            logItem.Timestamp = DateTime.UtcNow;
            logItem.Event = @event;

            data.Add(logItem);
        }

        public static void LogErrorEvent(this DataConnection data, Exception exc, IMailMessage message)
        {
            var logItem = data.CreateNew<IEventError>();

            logItem.Id = Guid.NewGuid();
            logItem.MailMessageId = message.Id;
            logItem.Timestamp = DateTime.UtcNow;
            logItem.Error = exc.ToString();

            data.Add(logItem);
        }
    }
}
