using System;

using Composite.C1Console.Workflow;
using Composite.Data;

using CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens;
using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Email.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed class EditMailTemplateWorkflow : Basic1StepDocumentWorkflow
    {
        public EditMailTemplateWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Email\\EditMailTemplate.xml") { }
        
        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("MailTemplate"))
            {
                return;
            }

            var dataToken = (DataEntityToken)EntityToken;
            var template = (IMailTemplate)dataToken.Data;

            Bindings.Add("MailTemplate", template);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var mailTemplate = GetBinding<IMailTemplate>("MailTemplate");

            using (var data = new DataConnection())
            {
                data.Update(mailTemplate);
            }

            var treeRefresher = CreateSpecificTreeRefresher();
            treeRefresher.PostRefreshMesseges(new MailTemplatesEntityToken());

            SetSaveStatus(true);
        }
    }
}
