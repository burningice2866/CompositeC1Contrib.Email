using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

using CompositeC1Contrib.Email.Data;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens
{
    [SecurityAncestorProvider(typeof(SentMailsAncestorProvider))]
    public class QueueFolderEntityToken : EntityToken
    {
        public override string Id => "QueueFolderEntityToken";

        public override string Source { get; }

        public override string Type { get; }

        public QueueFolderEntityToken(MailQueue queue, QueueFolder folderType)
        {
            Source = queue.Id.ToString();
            Type = folderType.ToString();
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            DoDeserialize(serializedEntityToken, out string type, out string source, out string _);

            var queue = MailQueuesFacade.GetMailQueue(Guid.Parse(source));
            var folderType = (QueueFolder)Enum.Parse(typeof(QueueFolder), type);

            return new QueueFolderEntityToken(queue, folderType);
        }
    }

    public class SentMailsAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            if (entityToken is QueueFolderEntityToken token)
            {
                yield return new MailQueueEntityToken(Guid.Parse(token.Source));
            }
        }
    }
}
