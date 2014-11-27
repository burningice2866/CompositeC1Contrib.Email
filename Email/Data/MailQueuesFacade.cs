﻿using System;
using System.Collections.Generic;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.Data
{
    public static class MailQueuesFacade
    {
        private static readonly IDictionary<Guid, MailQueue> MailQueues = new Dictionary<Guid, MailQueue>();
        private static readonly object QueuesLock = new object();

        public static readonly List<Type> MailClientTypes = new List<Type>();

        static MailQueuesFacade()
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                try
                {
                    var types = asm.GetTypes()
                        .Where(t => typeof(IMailClient).IsAssignableFrom(t)
                            && t.IsClass
                            && !t.IsAbstract).ToList();

                    MailClientTypes.AddRange(types);
                }
                catch { }
            }

            DataEvents<IMailQueue>.OnAfterAdd += MailWorker_OnAfterAdd;
            DataEvents<IMailQueue>.OnDeleted += MailWorker_OnDeleted;
            DataEvents<IMailQueue>.OnAfterUpdate += MailWorker_OnAfterUpdate;

            using (var data = new DataConnection())
            {
                var queues = data.Get<IMailQueue>();
                foreach (var q in queues)
                {
                    var queueModel = new MailQueue(q);

                    if (queueModel.Client == null)
                    {
                        q.Paused = true;
                        data.Update(q);

                        continue;
                    }

                    MailQueues.Add(q.Id, queueModel);
                }
            }
        }

        public static MailQueue GetMailQueue(string name)
        {
            return GetMailQueues().SingleOrDefault(q => q.Name == name);
        }

        public static IEnumerable<MailQueue> GetMailQueues()
        {
            return MailQueues.Values;
        }

        private static void MailWorker_OnAfterUpdate(object sender, DataEventArgs dataEventArgs)
        {
            var queue = (IMailQueue)dataEventArgs.Data;

            lock (QueuesLock)
            {
                MailQueues.Remove(queue.Id);

                var queueModel = new MailQueue(queue);
                if (queueModel.Client == null)
                {
                    return;
                }

                MailQueues.Add(queue.Id, queueModel);
            }
        }

        private static void MailWorker_OnDeleted(object sender, DataEventArgs dataEventArgs)
        {
            var queue = (IMailQueue)dataEventArgs.Data;

            lock (QueuesLock)
            {
                MailQueues.Remove(queue.Id);
            }
        }

        private static void MailWorker_OnAfterAdd(object sender, DataEventArgs dataEventArgs)
        {
            var queue = (IMailQueue)dataEventArgs.Data;

            var queueModel = new MailQueue(queue);
            if (queueModel.Client == null)
            {
                return;
            }

            lock (QueuesLock)
            {
                MailQueues.Add(queue.Id, queueModel);
            }
        }
    }
}
