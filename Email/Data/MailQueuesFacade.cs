using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Composite.Core.IO;
using Composite.Data;
using Composite.Data.DynamicTypes;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.Email.Data.Types;

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

        public static void Upgrade()
        {
            var descriptor = DynamicTypeManager.GetDataTypeDescriptor(typeof(IMailQueue));
            if (descriptor == null)
            {
                return;
            }

            using (var data = new DataConnection())
            {
                var queues = data.Get<IMailQueue>();
                foreach (var q in queues)
                {
                    var queueModel = new MailQueue
                    {
                        Id = q.Id,
                        Name = q.Name,
                        From = q.From,
                        Paused = q.Paused
                    };

                    var client = GetMailClient(q);
                    if (client != null)
                    {
                        queueModel.Client = client;
                    }

                    queueModel.Save();

                    data.Delete(q);
                }
            }

            DynamicTypeManager.DropStore(descriptor);
        }

        private static IMailClient GetMailClient(IMailQueue queue)
        {
            if (String.IsNullOrEmpty(queue.ClientType))
            {
                return null;
            }

            var type = Type.GetType(queue.ClientType);
            if (type == null)
            {
                return null;
            }

            if (type == typeof(DefaultMailClient))
            {
                return new ConfigurableSystemNetMailClient
                {
                    DeliveryMethod = queue.DeliveryMethod,
                    Host = queue.Host,
                    Port = queue.Port,
                    EnableSsl = queue.EnableSsl,
                    TargetName = queue.TargetName,

                    PickupDirectoryLocation = queue.PickupDirectoryLocation,

                    DefaultCredentials = queue.DefaultCredentials,
                    UserName = queue.UserName,
                    Password = queue.Password
                };
            }

            var queueConstructor = type.GetConstructor(new[] { typeof(IMailQueue) });
            if (queueConstructor != null)
            {
                return (IMailClient)queueConstructor.Invoke(new object[] { queue });
            }

            return (IMailClient)Activator.CreateInstance(type);
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
