using System;
using System.Linq;

using Composite.C1Console.Security;
using Composite.Data;
using Composite.Functions;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.FunctionProviders
{
    [SecurityAncestorProvider(typeof (StandardFunctionSecurityAncestorProvider))]
    public class EmailFunctionEntityToken : EntityToken
    {
        private readonly string _id;
        public override string Id
        {
            get { return _id; }
        }

        public override string Source
        {
            get { return String.Empty; }
        }

        public override string Type
        {
            get { return String.Empty; }
        }

        public EmailFunctionEntityToken(IMailTemplate template)
        {
            _id = template.Key;
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
                var template = data.Get<IMailTemplate>().Single(t => t.Key == id);

                return new EmailFunctionEntityToken(template);
            }
        }
    }
}