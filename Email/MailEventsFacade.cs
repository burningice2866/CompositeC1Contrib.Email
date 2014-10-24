using System;
using System.Collections.Generic;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Data;
using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    public class MailEventsFacade
    {
        public static IEnumerable<T> GetEventsOfType<T>(IMailTemplate template) where T : class, IEvent
        {
            using (var data = new DataConnection())
            {
                var messages = data.Get<ISentMailMessage>().Where(m => m.MailTemplateKey == template.Key);

                var q = from m in messages
                        join e in data.Get<T>() on m.Id equals e.MailMessageId
                        orderby e.Timestamp
                        select e;

                return q;
            }
        }

        public static IEnumerable<T> GetEventsOfType<T>(IMailMessage message) where T : class, IEvent
        {
            using (var data = new DataConnection())
            {
                return data.Get<T>().Where(e => e.MailMessageId == message.Id);
            }
        }

        public static IEnumerable<IEvent> GetEvents(IMailMessage message)
        {
            return GetEvents(message.Id);
        }

        public static IEnumerable<IEvent> GetEvents(Guid messageId)
        {
            return InheritanceDataFacade.GetData<IEvent>(l => l.MailMessageId == messageId).OrderBy(l => l.Timestamp);
        }

        public static void ClearEvents(IMailMessage message)
        {
            InheritanceDataFacade.Delete<IEvent>(l => l.MailMessageId == message.Id);
        }
    }
}
