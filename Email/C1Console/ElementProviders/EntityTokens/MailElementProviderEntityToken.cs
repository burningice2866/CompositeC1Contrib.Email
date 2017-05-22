using System;

using Composite.C1Console.Security;
using Composite.C1Console.Security.SecurityAncestorProviders;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens
{
    [SecurityAncestorProvider(typeof(NoAncestorSecurityAncestorProvider))]
    public class MailElementProviderEntityToken : EntityToken
    {
        public override string Id => "MailElementProviderEntityToken";

        public override string Source => String.Empty;

        public override string Type => String.Empty;

        public static EntityToken Deserialize(string serializedData)
        {
            return new MailElementProviderEntityToken();
        }

        public override string Serialize()
        {
            return String.Empty;
        }
    }
}
