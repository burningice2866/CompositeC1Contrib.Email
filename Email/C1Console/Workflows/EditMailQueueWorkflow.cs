using System;
using System.Linq;

using Composite.C1Console.Workflow;
using Composite.Data;

using CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens;
using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Email.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed class EditMailQueueWorkflow : Basic1StepDocumentWorkflow
    {
        public EditMailQueueWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Email\\EditMailQueue.xml") { }
        
        public override bool Validate()
        {
            var mailQueue = GetBinding<IMailQueue>("MailQueue");

            using (var data = new DataConnection())
            {
                var savedQueue = data.Get<IMailQueue>().Single(q => q.Id == mailQueue.Id);

                if (savedQueue.Name != mailQueue.Name)
                {
                    var nameExists = data.Get<IMailQueue>().Any(q => q.Name == mailQueue.Name);
                    if (nameExists)
                    {
                        ShowFieldMessage("MailQueue.Name", "Mail queue with this name already exists");

                        return false;
                    }
                }
            }

            return base.Validate();
        }
        
        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("MailQueue"))
            {
                return;
            }

            var dataToken = (DataEntityToken)EntityToken;
            var queue = (IMailQueue)dataToken.Data;

            Bindings.Add("MailQueue", queue);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var mailQueue = GetBinding<IMailQueue>("MailQueue");

            using (var data = new DataConnection())
            {
                data.Update(mailQueue);
            }

            var treeRefresher = CreateSpecificTreeRefresher();
            treeRefresher.PostRefreshMesseges(new MailQueuesEntityToken());

            SetSaveStatus(true);
        }
    }
}
