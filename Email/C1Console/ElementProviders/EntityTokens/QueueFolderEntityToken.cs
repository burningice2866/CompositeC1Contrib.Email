using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

using CompositeC1Contrib.Email.Data;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens
{
    [SecurityAncestorProvider(typeof(SentMailsAncestorProvider))]
    public class QueueFolderEntityToken : EntityToken
    {
        public override string Id
        {
            get { return "QueueFolderEntityToken"; }
        }

        private readonly string _source;
        public override string Source
        {
            get { return _source; }
        }

        private readonly string _type;
        public override string Type
        {
            get { return _type; }
        }

        public QueueFolderEntityToken(MailQueue queue, QueueFolder folderType)
        {
            _source = queue.Id.ToString();
            _type = folderType.ToString();
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
            var folderType = (QueueFolder)Enum.Parse(typeof(QueueFolder), type);

            return new QueueFolderEntityToken(queue, folderType);
        }
    }

    public class SentMailsAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var token = entityToken as QueueFolderEntityToken;
            if (token == null)
            {
                yield break;
            }

            yield return new MailQueueEntityToken(Guid.Parse(token.Source));
        }
    }
}
