using System;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    public static class Logger
    {
        public static void LogBasicEvent(this DataConnection data, string @event, IMailMessage message)
        {
            var logItem = data.CreateEvent<IEventBasic>(message);

            logItem.Event = @event;

            data.Add(logItem);
        }

        public static void LogErrorEvent(this DataConnection data, Exception exc, IMailMessage message)
        {
            var logItem = data.CreateEvent<IEventError>(message);

            logItem.Error = exc.ToString();

            data.Add(logItem);
        }

        public static int GetEventCount<T>(this DataConnection data, IMailMessage message, TimeSpan timeSpan) where T : class, IEvent
        {
            var timestamp = DateTime.UtcNow - timeSpan;

            return data.Get<T>().Count(e => e.MailMessageId == message.Id && e.Timestamp > timestamp);
        }

        private static T CreateEvent<T>(this DataConnection data, IMailMessage message) where T : class, IEvent
        {
            var logItem = data.CreateNew<T>();

            logItem.Id = Guid.NewGuid();
            logItem.MailMessageId = message.Id;
            logItem.Timestamp = DateTime.UtcNow;

            return logItem;
        }
    }
}
