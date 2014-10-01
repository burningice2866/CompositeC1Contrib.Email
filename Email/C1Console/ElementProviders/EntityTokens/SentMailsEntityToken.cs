using System;
using System.Collections.Generic;
using System.Linq;

using Composite.C1Console.Security;
using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

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

        public SentMailsEntityToken(IMailQueue queue)
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

            using (var data = new DataConnection())
            {
                var queue = data.Get<IMailQueue>().Single(q => q.Id == Guid.Parse(source));

                return new SentMailsEntityToken(queue);
            }
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

            using (var data = new DataConnection())
            {
                var queue = data.Get<IMailQueue>().Single(q => q.Id == new Guid(token.Source));

                yield return queue.GetDataEntityToken();
            }
        }
    }
}
