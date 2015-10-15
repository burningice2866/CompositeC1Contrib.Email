﻿using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

using CompositeC1Contrib.Email.Data;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens
{
    [SecurityAncestorProvider(typeof(SentMailsAncestorProvider))]
    public class SentMailsEntityToken : EntityToken
    {
        public override string Id
        {
            get { return "SentMailsEntityToken"; }
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

        public SentMailsEntityToken(MailQueue queue)
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

            return new SentMailsEntityToken(queue);
        }
    }

    public class SentMailsAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var token = entityToken as SentMailsEntityToken;
            if (token == null)
            {
                yield break;
            }

            yield return new MailQueueEntityToken(Guid.Parse(token.Source));
        }
    }
}
