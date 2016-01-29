using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

using Composite.Data;
using Composite.Data.DynamicTypes;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.Email.Data;
using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.ScheduledTasks;

using Owin;

namespace CompositeC1Contrib.Email
{
    public static class OwinExtensions
    {
        public static void UseCompositeC1ContribEmail(this IAppBuilder app, HttpConfiguration httpConfig, ScheduledTasksConfiguration scheduledTasksConfig)
        {
            UseCompositeC1ContribEmail(app, httpConfig, scheduledTasksConfig, null);
        }

        public static void UseCompositeC1ContribEmail(this IAppBuilder app, HttpConfiguration httpConfig, ScheduledTasksConfiguration scheduledTasksConfig, Action<IBootstrapperConfiguration> configurationAction)
        {
            Init();

            if (configurationAction != null)
            {
                var configuration = new BootstrapperConfiguration(httpConfig);

                configurationAction(configuration);
            }

            scheduledTasksConfig.AddBackgroundProcess(new MailBackgroundProcess());
        }

        private static void Init()
        {
            EnsureCreateStore();
            AddTemplates();

            MailQueuesFacade.Upgrade();
        }

        private static void EnsureCreateStore()
        {
            var dataTypes = new[]
            {
                typeof (IMailTemplate),

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
            template.Subject = String.Empty;
            template.Body = String.Empty;

            data.Add(template);
        }
    }
}