using System;
using System.Collections.Generic;

using Composite.C1Console.Security;
using CompositeC1Contrib.Email.Data;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens
{
    [SecurityAncestorProvider(typeof(QueuedMailsAncestorProvider))]
    public class QueuedMailsEntityToken : EntityToken
    {
        public override string Id
        {
            get { return "QueuedMailsEntityToken"; }
        }

        private readonly string _source;
        public override string Source
        {
            get { return _source; }
        }

        public override string Type
        {
            get { return String.Empty; }
        }

        public QueuedMailsEntityToken(MailQueue queue)
        {
            _source = queue.Id.ToString();
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            string type;
            string source;
            string id;

            DoDeserialize(serializedEntityToken, out type, out source, out id);

            var queue = MailQueuesFacade.GetMailQueue(Guid.Parse(source));

            return new QueuedMailsEntityToken(queue);
        }
    }

    public class QueuedMailsAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var token = entityToken as QueuedMailsEntityToken;
            if (token == null)
            {
                yield break;
            }

            yield return new MailQueueEntityToken(Guid.Parse(token.Source));
        }
    }
}
