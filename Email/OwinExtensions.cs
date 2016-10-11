using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.DynamicTypes;
using Composite.Data.Types;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.Email.Data;
using CompositeC1Contrib.Email.Data.Types;

using Owin;

namespace CompositeC1Contrib.Email
{
    public static class OwinExtensions
    {
        public static void UseCompositeC1ContribEmail(this IAppBuilder app)
        {
            UseCompositeC1ContribEmail(app, null);
        }

        public static void UseCompositeC1ContribEmail(this IAppBuilder app, Action<IBootstrapperConfiguration> configurationAction)
        {
            Init();

            if (configurationAction != null)
            {
                var configuration = new BootstrapperConfiguration();

                configurationAction(configuration);
            }
        }

        private static void Init()
        {
            EnsureCreateStore();
            AddTemplates();
        }

        private static void EnsureCreateStore()
        {
            var dataTypes = new[]
            {
                typeof (IMailTemplate), typeof(IMailTemplateContent),

                typeof (IQueuedMailMessage), typeof (ISentMailMessage), typeof(IBadMailMessage),

                typeof (IEventBasic), typeof (IEventError), typeof (IEventOpen), typeof (IEventClick)
            };

            foreach (var t in dataTypes)
            {
                DynamicTypeManager.EnsureCreateStore(t);
            }
        }

        private static void AddTemplates()
        {
            using (var data = new DataConnection())
            {
                var mailTemplates = data.Get<IMailTemplate>().ToDictionary(t => t.Key);

                AddTemplatesFromProviders(data, mailTemplates);
                AddTemplatesFromModels(data, mailTemplates);
            }
        }

        private static void AddTemplatesFromProviders(DataConnection data, IDictionary<string, IMailTemplate> mailTemplates)
        {
            var providers = CompositionContainerFacade.GetExportedValues<IMailTemplatesProvider>().ToList();

            foreach (var provider in providers)
            {
                var templates = provider.GetTemplates();

                foreach (var template in templates)
                {
                    if (mailTemplates.ContainsKey(template.Key))
                    {
                        continue;
                    }

                    AddMailTemplate(template.Key, template.ModelType, data);
                }
            }
        }

        private static void AddTemplatesFromModels(DataConnection data, IDictionary<string, IMailTemplate> mailTemplates)
        {
            var models = CompositionContainerFacade.GetExportedTypes<object>(b =>
            {
                var partBuilder = b.ForTypesMatching(t => t.IsClass && !t.IsAbstract && t.GetCustomAttribute<MailModelAttribute>() != null);

                partBuilder.Export<object>();
            });

            foreach (var type in models)
            {
                var attribute = type.GetCustomAttribute<MailModelAttribute>();

                if (mailTemplates.ContainsKey(attribute.Key))
                {
                    continue;
                }

                AddMailTemplate(attribute.Key, type, data);
            }
        }

        private static void AddMailTemplate(string key, Type type, DataConnection data)
        {
            var template = data.CreateNew<IMailTemplate>();

            template.Key = key;
            template.ModelType = type.AssemblyQualifiedName;

            data.Add(template);
        }

        private static void UpgradeTemplates()
        {
            var defaultCulture = CultureInfo.CurrentCulture;

            using (var data = new DataConnection())
            {
                var systemLocale = data.Get<ISystemActiveLocale>().SingleOrDefault(s => s.IsDefault);
                if (systemLocale != null)
                {
                    defaultCulture = CultureInfo.GetCultureInfo(systemLocale.CultureName);
                }
            }

            using (var data = new DataConnection(defaultCulture))
            {
                var templates = data.Get<IMailTemplate>().ToList();
                foreach (var template in templates)
                {
                    var subject = template.Subject;
                    var body = template.Body;

                    if (subject == null && body == null)
                    {
                        continue;
                    }

                    var content = data.Get<IMailTemplateContent>().SingleOrDefault(t => t.TemplateKey == template.Key);
                    if (content == null)
                    {
                        content = data.CreateNew<IMailTemplateContent>();

                        content.Id = Guid.NewGuid();
                        content.TemplateKey = template.Key;

                        content = data.Add(content);
                    }

                    content.Subject = subject ?? String.Empty;
                    content.Body = body ?? new XhtmlDocument().ToString();

                    data.Update(content);

                    template.Subject = null;
                    template.Body = null;

                    data.Update(template);
                }
            }
        }
    }
}