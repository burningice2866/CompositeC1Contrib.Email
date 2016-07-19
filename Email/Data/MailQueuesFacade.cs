using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Composite.Core.IO;

using CompositeC1Contrib.Composition;

namespace CompositeC1Contrib.Email.Data
{
    public static class MailQueuesFacade
    {
        private static IDictionary<Guid, MailQueue> _mailQueues;

        private static readonly string QueuesFile = Path.Combine(MailsFacade.BasePath, "queues.xml");

        public static readonly List<Type> MailClientTypes;

        static MailQueuesFacade()
        {
            MailClientTypes = CompositionContainerFacade.GetExportedTypes<IMailClient>().ToList();
        }

        public static MailQueue GetMailQueue(Guid id)
        {
            return GetMailQueues().SingleOrDefault(q => q.Id == id);
        }

        public static MailQueue GetMailQueue(string name)
        {
            return GetMailQueues().SingleOrDefault(q => q.Name == name);
        }

        public static IEnumerable<MailQueue> GetMailQueues()
        {
            if (_mailQueues == null)
            {
                var queues = !C1File.Exists(QueuesFile) ? new XElement("queues") : XElement.Load(QueuesFile);

                _mailQueues = queues.Elements("queue").Select(MailQueue.Load).ToDictionary(queue => queue.Id);
            }

            return _mailQueues.Values;
        }

        public static void Save(XElement queues)
        {
            queues.Save(QueuesFile);

            _mailQueues = null;
        }

        public static XElement GetFile()
        {
            return C1File.Exists(QueuesFile) ? XElement.Load(QueuesFile) : new XElement("queues");
        }
    }
}
