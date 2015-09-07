using System;
using System.Collections.Generic;
using System.Linq;

using Composite.Data;
using Composite.Data.DynamicTypes;

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
            DynamicTypeManager.EnsureCreateStore(typeof(IMailQueue));
            DynamicTypeManager.EnsureCreateStore(typeof(IQueuedMailMessage));
            DynamicTypeManager.EnsureCreateStore(typeof(ISentMailMessage));
            DynamicTypeManager.EnsureCreateStore(typeof(IMailTemplate));

            DynamicTypeManager.EnsureCreateStore(typeof(IEventBasic));
            DynamicTypeManager.EnsureCreateStore(typeof(IEventError));
            DynamicTypeManager.EnsureCreateStore(typeof(IEventOpen));
            DynamicTypeManager.EnsureCreateStore(typeof(IEventClick));

            using (var data = new DataConnection())
            {
                var mailTemplates = data.Get<IMailTemplate>().ToDictionary(t => t.Key);

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        var types = assembly.GetTypes();

                        AddTemplatesFromProviders(data, mailTemplates, types);
                        AddTemplatesFromModels(data, mailTemplates, types);
                    }
                    catch { }
                }
            }

            MailWorker.Initialize();
        }

        private static void AddTemplatesFromProviders(DataConnection data, IDictionary<string, IMailTemplate> mailTemplates, IEnumerable<Type> types)
        {
            foreach (var type in types.Where(t => t.IsClass && !t.IsAbstract && typeof(IMailTemplatesProvider).IsAssignableFrom(t)))
            {
                var provicer = (IMailTemplatesProvider)Activator.CreateInstance(type);
                var templates = provicer.GetTemplates();

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

        private static void AddTemplatesFromModels(DataConnection data, IDictionary<string, IMailTemplate> mailTemplates, IEnumerable<Type> types)
        {
            foreach (var type in types.Where(t => t.IsClass && !t.IsAbstract))
            {
                var attribute = type.GetCustomAttributes(typeof(MailModelAttribute), false).Cast<MailModelAttribute>().FirstOrDefault();
                if (attribute == null)
                {
                    continue;
                }

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
            template.Subject = String.Empty;
            template.Body = String.Empty;

            data.Add(template);
        }
    }
}