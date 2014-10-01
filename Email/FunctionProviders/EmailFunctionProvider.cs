using System.Collections.Generic;

using Composite.Data;
using Composite.Functions;
using Composite.Functions.Plugins.FunctionProvider;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.FunctionProviders
{
    public class EmailFunctionProvider : IFunctionProvider
    {
        public FunctionNotifier FunctionNotifier { private get; set; }

        public IEnumerable<IFunction> Functions
        {
            get
            {
                using (var data = new DataConnection())
                {
                    var templates = data.Get<IMailTemplate>();
                    foreach (var template in templates)
                    {
                        yield return new EmailFunction(template);
                    }
                }
            }
        }
    }
}
