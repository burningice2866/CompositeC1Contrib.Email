﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementProvider;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Core.WebClient;
using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Composition;
using CompositeC1Contrib.Email.C1Console.ElementProviders.Actions;
using CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens;
using CompositeC1Contrib.Email.C1Console.Workflows;
using CompositeC1Contrib.Email.Data;
using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.C1Console.ElementProviders
{
    public class MailElementProvider : IHooklessElementProvider, IAuxiliarySecurityAncestorProvider
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup(ActionGroupPriority.PrimaryHigh);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = true, ActionGroup = ActionGroup };

        private static readonly IList<IElementActionProvider> ElementActionProviders;

        private const string StatisticsUrlTemplate = "InstalledPackages/CompositeC1Contrib.Email/statistics.aspx?template={0}";
        private const string LogUrlTemplate = "InstalledPackages/CompositeC1Contrib.Email/log.aspx?view={0}&queue={1}&template={2}";

        public ElementProviderContext Context { get; set; }

        static MailElementProvider()
        {
            var actionProviders = CompositionContainerFacade.GetExportedValues<IElementActionProvider>().ToList();

            ElementActionProviders = actionProviders;
        }

        public MailElementProvider()
        {
            AuxiliarySecurityAncestorFacade.AddAuxiliaryAncestorProvider<DataEntityToken>(this);

            MailsFacade.Queued += (sender, args) => UpdateQueues();
        }

        public static void UpdateQueues()
        {
            using (var data = new DataConnection())
            {
                var consoleIds = data.Get<IUserConsoleInformation>().Select(u => u.ConsoleId).ToList();

                foreach (var id in consoleIds)
                {
                    Util.UpdateQueuesCount(id);
                }
            }
        }

        public IEnumerable<Element> GetChildren(EntityToken entityToken, SearchToken searchToken)
        {
            var elements = new List<Element>();

            if (entityToken is MailQueuesEntityToken)
            {
                var queues = MailQueuesFacade.GetMailQueues();
                foreach (var queue in queues)
                {
                    var label = queue.Name;

                    if (queue.Paused)
                    {
                        label += " (paused)";
                    }

                    var elementHandle = Context.CreateElementHandle(new MailQueueEntityToken(queue.Id));
                    var element = new Element(elementHandle)
                    {
                        VisualData = new ElementVisualizedData
                        {
                            Label = label,
                            ToolTip = label,
                            HasChildren = true,
                            Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                            OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                        }
                    };

                    var clientType = queue.Client.GetType();
                    var editWorkflowAttribute = clientType.GetCustomAttribute<EditWorkflowAttribute>();
                    var editWorkflowType = editWorkflowAttribute == null ? typeof(EditMailQueueWorkflow) : editWorkflowAttribute.EditWorkflowType;

                    var editActionToken = new WorkflowActionToken(editWorkflowType, new[] { PermissionType.Edit });
                    element.AddAction(new ElementAction(new ActionHandle(editActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Edit",
                            ToolTip = "Edit",
                            Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                            ActionLocation = ActionLocation
                        }
                    });

                    var deleteActionToken = new ConfirmWorkflowActionToken("Are you sure?", typeof(DeleteMailQueueActionToken));
                    element.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Delete",
                            ToolTip = "Delete",
                            Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                            ActionLocation = ActionLocation
                        }
                    });

                    var toggleStateActionToken = new ToggleMailQueueStateActionToken(queue.Id);
                    var toggleLabel = queue.Paused ? "Resume" : "Pause";
                    var toggleIcon = queue.Paused ? "accept" : "generated-type-data-delete";

                    element.AddAction(new ElementAction(new ActionHandle(toggleStateActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = toggleLabel,
                            ToolTip = toggleLabel,
                            Icon = new ResourceHandle("Composite.Icons", toggleIcon),
                            ActionLocation = ActionLocation
                        }
                    });

                    elements.Add(element);
                }
            }

            var queueToken = entityToken as MailQueueEntityToken;
            if (queueToken != null)
            {
                var id = Guid.Parse(queueToken.Id);

                var queue = MailQueuesFacade.GetMailQueue(id);
                if (queue != null)
                {
                    var queueFolder = AddFolder<IQueuedMailMessage>(queue, QueueFolder.Queued);
                    var sentFolder = AddFolder<ISentMailMessage>(queue, QueueFolder.Sent);
                    var badMailFolder = AddFolder<IBadMailMessage>(queue, QueueFolder.BadMail);

                    elements.Add(queueFolder);
                    elements.Add(sentFolder);
                    elements.Add(badMailFolder);
                }
            }

            if (entityToken is MailTemplatesEntityToken)
            {
                foreach (var el in GetNamespaceAndTemplateElements(Context, String.Empty))
                {
                    elements.Add(el);
                }
            }

            var folderToken = entityToken as NamespaceFolderEntityToken;
            if (folderToken != null)
            {
                foreach (var el in GetNamespaceAndTemplateElements(Context, folderToken.Namespace))
                {
                    elements.Add(el);
                }
            }

            if (entityToken is MailElementProviderEntityToken)
            {
                var queuesElementHandle = Context.CreateElementHandle(new MailQueuesEntityToken());
                var queuesElement = new Element(queuesElementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = "Queues",
                        ToolTip = "Queues",
                        HasChildren = MailQueuesFacade.GetMailQueues().Any(),
                        Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                        OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                    }
                };

                var createQueueActionToken = new WorkflowActionToken(typeof(CreateMailQueueWorkflow));
                queuesElement.AddAction(new ElementAction(new ActionHandle(createQueueActionToken))
                {
                    VisualData = new ActionVisualizedData
                    {
                        Label = "Add queue",
                        ToolTip = "Add queue",
                        Icon = new ResourceHandle("Composite.Icons", "generated-type-data-add"),
                        ActionLocation = ActionLocation
                    }
                });

                elements.Add(queuesElement);

                var templatesElementHandle = Context.CreateElementHandle(new MailTemplatesEntityToken());
                var templatesElement = new Element(templatesElementHandle)
                {
                    VisualData = new ElementVisualizedData
                    {
                        Label = "Templates",
                        ToolTip = "Templates",
                        HasChildren = GetTemplates(String.Empty).Any(),
                        Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                        OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                    }
                };

                elements.Add(templatesElement);
            }

            foreach (var el in elements)
            {
                var token = el.ElementHandle.EntityToken;

                foreach (var provider in ElementActionProviders.Where(p => p.IsProviderFor(token)))
                {
                    provider.AddActions(el);
                }
            }

            return elements;
        }

        private Element AddFolder<T>(MailQueue queue, QueueFolder folderType) where T : class, IMailMessage
        {
            var label = folderType.ToString();

            var queuedCount = GetMessagesCount<T>(queue);
            if (queuedCount > 0)
            {
                label += " (" + queuedCount + ")";
            }

            var elementHandle = Context.CreateElementHandle(new QueueFolderEntityToken(queue, folderType));
            var element = new Element(elementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = label,
                    ToolTip = folderType.ToString(),
                    HasChildren = false,
                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                }
            };

            AddViewLogAction(folderType, queue, null, element);

            return element;
        }

        private static IEnumerable<Element> GetNamespaceAndTemplateElements(ElementProviderContext context, string ns)
        {
            var templates = GetTemplates(ns);

            var folders = new List<string>();
            var elements = new List<Element>();

            foreach (var template in templates)
            {
                var label = template.Key;

                if (!String.IsNullOrEmpty(ns))
                {
                    label = label.Remove(0, ns.Length + 1);
                }

                var split = label.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 1)
                {
                    var folder = split[0];

                    if (!folders.Contains(folder))
                    {
                        folders.Add(folder);
                    }
                }
                else if (split.Length == 1)
                {
                    var token = template.GetDataEntityToken();

                    var elementHandle = context.CreateElementHandle(token);
                    var element = new Element(elementHandle)
                    {
                        VisualData = new ElementVisualizedData
                        {
                            Label = label,
                            ToolTip = label,
                            HasChildren = false,
                            Icon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-closed-root"),
                            OpenedIcon = ResourceHandle.BuildIconFromDefaultProvider("localization-element-opened-root")
                        }
                    };

                    var editActionToken = new WorkflowActionToken(typeof(EditMailTemplateWorkflow), new[] { PermissionType.Edit });
                    element.AddAction(new ElementAction(new ActionHandle(editActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Edit",
                            ToolTip = "Edit",
                            Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                            ActionLocation = ActionLocation
                        }
                    });

                    var deleteActionToken = new ConfirmWorkflowActionToken("Are you sure?", typeof(DeleteMailTemplateActionToken));
                    element.AddAction(new ElementAction(new ActionHandle(deleteActionToken))
                    {
                        VisualData = new ActionVisualizedData
                        {
                            Label = "Delete",
                            ToolTip = "Delete",
                            Icon = new ResourceHandle("Composite.Icons", "generated-type-data-delete"),
                            ActionLocation = ActionLocation
                        }
                    });

                    AddViewLogAction(QueueFolder.Sent, null, template, element);
                    AddViewStatisticsAction(template, element);

                    elements.Add(element);
                }
            }

            foreach (var folder in folders.OrderBy(f => f))
            {
                var handleNamespace = folder;
                if (!String.IsNullOrEmpty(ns))
                {
                    handleNamespace = ns + "." + handleNamespace;
                }

                var folderElement = NamespaceFolderEntityToken.CreateElement(context, folder, handleNamespace);

                yield return folderElement;
            }

            foreach (var el in elements)
            {
                yield return el;
            }
        }

        private static void AddViewLogAction(QueueFolder folder, MailQueue queue, IMailTemplate template, Element element)
        {
            var sQueue = queue?.Id.ToString() ?? String.Empty;
            var sTemplate = template == null ? String.Empty : template.Key;

            var url = String.Format(LogUrlTemplate, folder, sQueue, sTemplate);

            var queuedUrlAction = new UrlActionToken("View log", UrlUtils.ResolveAdminUrl(url), new[] { PermissionType.Read });
            element.AddAction(new ElementAction(new ActionHandle(queuedUrlAction))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "View log",
                    ToolTip = "View log",
                    Icon = new ResourceHandle("Composite.Icons", "log"),
                    ActionLocation = ActionLocation
                }
            });
        }

        private static void AddViewStatisticsAction(IMailTemplate template, Element element)
        {
            var url = String.Format(StatisticsUrlTemplate, template.Key);

            var queuedUrlAction = new UrlActionToken("View statistics", UrlUtils.ResolveAdminUrl(url), new[] { PermissionType.Read });
            element.AddAction(new ElementAction(new ActionHandle(queuedUrlAction))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "View statistics",
                    ToolTip = "View statistics",
                    Icon = new ResourceHandle("Composite.Icons", "log"),
                    ActionLocation = ActionLocation
                }
            });
        }

        public IEnumerable<Element> GetRoots(SearchToken searchToken)
        {
            var elementHandle = Context.CreateElementHandle(new MailElementProviderEntityToken());
            var rootElement = new Element(elementHandle)
            {
                VisualData = new ElementVisualizedData
                {
                    Label = "Email",
                    ToolTip = "Email",
                    HasChildren = true,
                    Icon = new ResourceHandle("Composite.Icons", "localization-element-closed-root"),
                    OpenedIcon = new ResourceHandle("Composite.Icons", "localization-element-opened-root")
                }
            };

            return new[] { rootElement };
        }

        public Dictionary<EntityToken, IEnumerable<EntityToken>> GetParents(IEnumerable<EntityToken> entityTokens)
        {
            var dictionary = new Dictionary<EntityToken, IEnumerable<EntityToken>>();
            foreach (var token in entityTokens)
            {
                var dataToken = token as DataEntityToken;

                var template = dataToken?.Data as IMailTemplate;
                if (template == null)
                {
                    continue;
                }

                var parts = template.Key.Split('.');
                var ns = String.Join(".", parts.Take(parts.Length - 1));

                dictionary.Add(token, new[] { new NamespaceFolderEntityToken(ns) });
            }

            return dictionary;
        }

        private static IEnumerable<IMailTemplate> GetTemplates(string ns)
        {
            using (var data = new DataConnection())
            {
                return data.Get<IMailTemplate>().Where(t => t.Key.StartsWith(ns)).OrderBy(t => t.Key);
            }
        }

        private static int GetMessagesCount<T>(MailQueue queue) where T : class, IMailMessage
        {
            using (var data = new DataConnection())
            {
                return data.Get<T>().Count(m => m.QueueId == queue.Id);
            }
        }
    }
}