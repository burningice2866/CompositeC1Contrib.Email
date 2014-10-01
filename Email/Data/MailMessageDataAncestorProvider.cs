using System;
using System.Linq;

using Composite.Data;
using Composite.Data.Hierarchy;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.Data
{
    public class MailMessageDataAncestorProvider : IDataAncestorProvider
    {
        public IData GetParent(IData data)
        {
            var mailMessage = data as IQueuedMailMessage;
            if (mailMessage == null)
            {
                throw new ArgumentException("Invalid data type", "data");
            }

            using (var conn = new DataConnection())
            {
                return conn.Get<IMailQueue>().Single(q => q.Id == mailMessage.QueueId);
            }
        }
    }
}
