using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

using CompositeC1Contrib.Email.Data;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens
{
    [SecurityAncestorProvider(typeof(MailQueueAncestorProvider))]
    public class MailQueueEntityToken : EntityToken
    {
        public override string Id { get; }

        public override string Source => String.Empty;

        public override string Type => String.Empty;

        public MailQueueEntityToken(Guid id)
        {
            Id = id.ToString();
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public MailQueue GetQueue()
        {
            var id = Guid.Parse(Id);

            return MailQueuesFacade.GetMailQueue(id);
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            string type;
            string source;
            string id;

            DoDeserialize(serializedEntityToken, out type, out source, out id);

            return new MailQueueEntityToken(Guid.Parse(id));
        }
    }

    public class MailQueueAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var token = entityToken as MailQueueEntityToken;
            if (token == null)
            {
                yield break;
            }

            yield return new MailQueuesEntityToken();
        }
    }
}
