using System.ComponentModel.Composition;

using Composite.C1Console.Elements;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Data;

using CompositeC1Contrib.Email.C1Console.ElementProviders;
using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.GoogleAnalytics
{
    [Export(typeof(IElementActionProvider))]
    public class MailTemplateActionProvider : IElementActionProvider
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup("Default", ActionGroupPriority.PrimaryLow);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = false, ActionGroup = ActionGroup };

        public bool IsProviderFor(EntityToken entityToken)
        {
            var dataEntityToken = entityToken as DataEntityToken;
            if (dataEntityToken == null)
            {
                return false;
            }

            var template = dataEntityToken.Data as IMailTemplate;
            if (template == null)
            {
                return false;
            }

            return true;
        }

        public void AddActions(Element element)
        {
            var editActionToken = new WorkflowActionToken(typeof(EditGoogleAnalyticsMailTemplateSettingsWorkFlow));

            element.AddAction(new ElementAction(new ActionHandle(editActionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Google Analytics settings",
                    ToolTip = "Google Analytics settings",
                    Icon = new ResourceHandle("Composite.Icons", "generated-type-data-edit"),
                    ActionLocation = ActionLocation
                }
            });
        }
    }
}