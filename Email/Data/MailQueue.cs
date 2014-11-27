using System;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.Data
{
    public class MailQueue
    {
        private readonly IMailQueue _iMailQueue;

        public Guid Id
        {
            get { return _iMailQueue.Id; }
        }

        public string Name
        {
            get { return _iMailQueue.Name; }
        }

        public string From
        {
            get { return _iMailQueue.From; }
        }

        public bool Paused
        {
            get { return _iMailQueue.Paused; }
        }

        public IMailClient Client { get; private set; }

        public MailQueue(IMailQueue iMailQueue)
        {
            _iMailQueue = iMailQueue;
            Client = GetMailClient(iMailQueue);
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

            var queueConstructor = type.GetConstructor(new [] { typeof(IMailQueue) });
            if (queueConstructor != null)
            {
                return (IMailClient)queueConstructor.Invoke(new object[] { queue });
            }

            return (IMailClient)Activator.CreateInstance(type);
        }
    }
}
