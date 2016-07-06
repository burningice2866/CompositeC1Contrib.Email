using System;
using System.Linq;

using Composite.C1Console.Workflow;

using CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens;
using CompositeC1Contrib.Email.Data;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Email.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public class EditMailQueueWorkflow : Basic1StepDocumentWorkflow
    {
        public EditMailQueueWorkflow() : this("\\InstalledPackages\\CompositeC1Contrib.Email\\EditMailQueue.xml") { }

        protected EditMailQueueWorkflow(string formDefinitionFile) : base(formDefinitionFile) { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Name"))
            {
                return;
            }

            var queueToken = (MailQueueEntityToken)EntityToken;
            var queue = queueToken.GetQueue();

            Bindings.Add("Name", queue.Name);
            Bindings.Add("From", queue.From);

            foreach (var prop in queue.Client.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var value = prop.GetValue(queue.Client);

                Bindings.Add(prop.Name, value);
            }
        }

        public override bool Validate()
        {
            var queueToken = (MailQueueEntityToken)EntityToken;
            var queue = queueToken.GetQueue();

            var name = GetBinding<string>("Name");

            if (queue.Name != name)
            {
                var nameExists = MailQueuesFacade.GetMailQueue(name) != null;
                if (nameExists)
                {
                    ShowFieldMessage("MailQueue.Name", "Mail queue with this name already exists");

                    return false;
                }
            }

            return base.Validate();
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var queueToken = (MailQueueEntityToken)EntityToken;
            var queue = queueToken.GetQueue();

            var name = GetBinding<string>("Name");
            var from = GetBinding<string>("From");

            queue.Name = name;
            queue.From = from;

            foreach (var prop in queue.Client.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var value = GetBinding<object>(prop.Name);

                prop.SetValue(queue.Client, value);
            }

            queue.Save();

            var treeRefresher = CreateSpecificTreeRefresher();
            treeRefresher.PostRefreshMesseges(new MailQueuesEntityToken());

            SetSaveStatus(true);
        }
    }
}
