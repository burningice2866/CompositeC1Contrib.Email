using System;
using System.Collections.Generic;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens
{
    [SecurityAncestorProvider(typeof(MailTemplatesAncestorProvider))]
    public class MailTemplatesEntityToken : EntityToken
    {
        public override string Id
        {
            get { return "MailTemplatesEntityToken"; }
        }

        public override string Source
        {
            get { return String.Empty; }
        }

        public override string Type
        {
            get { return String.Empty; }
        }

        public override string Serialize()
        {
            return DoSerialize();
        }

        public static EntityToken Deserialize(string serializedEntityToken)
        {
            return new MailTemplatesEntityToken();
        }
    }

    public class MailTemplatesAncestorProvider : ISecurityAncestorProvider
    {
        public IEnumerable<EntityToken> GetParents(EntityToken entityToken)
        {
            var token = entityToken as MailTemplatesEntityToken;
            if (token == null)
            {
                yield break;
            }

            yield return new MailElementProviderEntityToken();
        }
    }
}
