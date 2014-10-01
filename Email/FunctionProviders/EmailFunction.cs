using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Composite.C1Console.Security;
using Composite.Core.Xml;
using Composite.Functions;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.FunctionProviders
{
    public class EmailFunction : IFunction
    {
        private readonly IMailTemplate _template;
        private readonly string _ns = "Emails";
        private readonly string _name;

        public EntityToken EntityToken
        {
            get { return new EmailFunctionEntityToken(_template); }
        }

        public string Namespace
        {
            get { return _ns; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return String.Empty; }
        }

        public IEnumerable<ParameterProfile> ParameterProfiles
        {
            get
            {
                yield return new ParameterProfile("From", typeof(string), false, new ConstantValueProvider(_template.From), StandardWidgetFunctions.TextBoxWidget, "From address", new HelpDefinition("From address"));
                yield return new ParameterProfile("To", typeof(string), false, new ConstantValueProvider(_template.To), StandardWidgetFunctions.TextBoxWidget, "To address", new HelpDefinition("To address"));
                yield return new ParameterProfile("Cc", typeof(string), false, new ConstantValueProvider(_template.Cc), StandardWidgetFunctions.TextBoxWidget, "Cc address", new HelpDefinition("Cc address"));
                yield return new ParameterProfile("Bcc", typeof(string), false, new ConstantValueProvider(_template.Bcc), StandardWidgetFunctions.TextBoxWidget, "Bcc address", new HelpDefinition("Bcc address"));
                yield return new ParameterProfile("Subject", typeof(string), false, new ConstantValueProvider(_template.Subject), StandardWidgetFunctions.TextBoxWidget, "Subject", new HelpDefinition("Subject"));

                var xhtmlWidget = StandardWidgetFunctions.VisualXhtmlDocumentEditorWidget;

                var modelType = String.IsNullOrEmpty(_template.ModelType) ? null : Type.GetType(_template.ModelType);
                if (modelType != null)
                {
                    var element = new XElement(Namespaces.Function10 + "widgetfunction",
                        new XAttribute("name", "Composite.Widgets.XhtmlDocument.VisualXhtmlEditor"),
                        new XElement(Namespaces.Function10 + "param", new XAttribute("name", "ClassConfigurationName"),
                            "common"),
                        new XElement(Namespaces.Function10 + "param", new XAttribute("name", "EmbedableFieldsType"),
                            modelType.AssemblyQualifiedName)
                        );

                    xhtmlWidget = new WidgetFunctionProvider(element);
                }

                yield return new ParameterProfile("Body", typeof(XhtmlDocument), false, new ConstantValueProvider(_template.Body), xhtmlWidget, "Body", new HelpDefinition("Body"));

                if (modelType != null)
                {
                    var props = modelType.GetProperties().Where(p => p.CanRead && p.CanWrite);
                    foreach (var prop in props)
                    {
                        var widgetProvider = StandardWidgetFunctions.GetDefaultWidgetFunctionProviderByType(prop.PropertyType);
                        var valueProvider = new NoValueValueProvider();
                        var help = new HelpDefinition(prop.Name);

                        var profile = new ParameterProfile(prop.Name, prop.PropertyType, true, valueProvider,
                            widgetProvider, prop.Name, help);

                        yield return profile;
                    }
                }
            }
        }

        public Type ReturnType
        {
            get { return typeof(object); }
        }

        public EmailFunction(IMailTemplate template)
        {
            _template = template;

            if (_template.Key.Contains("."))
            {
                var split = _template.Key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                _ns = _ns + "." + String.Join(".", split.Take(split.Length - 1));
                _name = split.Last();
            }
            else
            {
                _name = _template.Key;
            }
        }

        public object Execute(ParameterList parameters, FunctionContextContainer context)
        {
            var modelType = String.IsNullOrEmpty(_template.ModelType) ? null : Type.GetType(_template.ModelType);
            if (modelType != null)
            {
                var model = Activator.CreateInstance(modelType);

                foreach (var itm in parameters.AllParameterNames)
                {
                    var prop = modelType.GetProperty(itm);
                    if (prop == null || !prop.CanWrite)
                    {
                        continue;
                    }

                    var value = parameters.GetParameter(itm);

                    prop.SetValue(model, value);
                }

                MailsFacade.BuildMessageAndEnqueue(model);
            }
            else
            {
                var obj = new
                {
                    From = parameters.GetParameter<string>("From"),
                    To = parameters.GetParameter<string>("To"),
                    Cc = parameters.GetParameter<string>("Cc"),
                    Bcc = parameters.GetParameter<string>("Bcc"),
                    Subject = parameters.GetParameter<string>("Subject"),
                    Body = parameters.GetParameter<string>("Body")
                };

                using (var message = MailModelsFacade.BuildMailMessage(_template, obj))
                {
                    MailsFacade.EnqueueMessage(message);
                }
            }

            return null;
        }
    }
}
