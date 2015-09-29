using System.Collections.Generic;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

using Composite.C1Console.Elements;
using Composite.C1Console.Elements.Plugins.ElementActionProvider;
using Composite.C1Console.Security;
using Composite.C1Console.Workflow;
using Composite.Core.ResourceSystem;
using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.GoogleAnalytics
{
    [ConfigurationElementType(typeof(NonConfigurableElementActionProvider))]
    public class GoogleAnalyticsActionProvider : IElementActionProvider
    {
        private static readonly ActionGroup ActionGroup = new ActionGroup("Default", ActionGroupPriority.PrimaryLow);
        private static readonly ActionLocation ActionLocation = new ActionLocation { ActionType = ActionType.Add, IsInFolder = false, IsInToolbar = false, ActionGroup = ActionGroup };

        public IEnumerable<ElementAction> GetActions(EntityToken entityToken)
        {
            var dataEntityToken = entityToken as DataEntityToken;
            if (dataEntityToken == null)
            {
                yield break;
            }

            var template = dataEntityToken.Data as IMailTemplate;
            if (template == null)
            {
                yield break;
            }

            var actionToken = new WorkflowActionToken(typeof(EditGoogleAnalyticsMailTemplateSettingsWorkFlow));

            yield return new ElementAction(new ActionHandle(actionToken))
            {
                VisualData = new ActionVisualizedData
                {
                    Label = "Google Analytics settings",
                    ToolTip = "Google Analytics settings",
                    Icon = new ResourceHandle("Composite.Icons", "edit"),
                    ActionLocation = ActionLocation
                }
            };
        }
    }
}