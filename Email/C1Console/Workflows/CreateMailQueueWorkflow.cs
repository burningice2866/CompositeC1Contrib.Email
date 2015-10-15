using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CompositeC1Contrib.Email.C1Console.ElementProviders.EntityTokens;

using CompositeC1Contrib.Email.Data;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Email.C1Console.Workflows
{
    public sealed class CreateMailQueueWorkflow : Basic1StepDialogWorkflow
    {
        public CreateMailQueueWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Email\\CreateMailQueue.xml") { }

        public static Dictionary<string, string> GetMailClientTypes()
        {
            var query = from t in MailQueuesFacade.MailClientTypes
                        let obsolete = t.GetCustomAttribute<ObsoleteAttribute>()
                        where obsolete == null
                        select t;

            return query.ToDictionary(t => t.AssemblyQualifiedName, t => t.Name);
        }

        public override bool Validate()
        {
            var name = GetBinding<string>("Name");

            var nameExists = MailQueuesFacade.GetMailQueues().Any(q => q.Name == name);
            if (nameExists)
            {
                ShowFieldMessage("Name", "Mail queue with this name already exists");

                return false;
            }

            return base.Validate();
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Name"))
            {
                return;
            }

            Bindings.Add("Name", String.Empty);
            Bindings.Add("ClientType", typeof(SystemNetMailClient).AssemblyQualifiedName);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var name = GetBinding<string>("Name");
            var clientType = GetBinding<string>("ClientType");

            var type = MailQueuesFacade.MailClientTypes.Single(t => t.AssemblyQualifiedName == clientType);
            var client = (IMailClient)Activator.CreateInstance(type);

            var mailQueue = new MailQueue
            {
                Id = Guid.NewGuid(),
                Name = name,
                Paused = true,
                Client = client
            };

            mailQueue.Save();

            var newQueueEntityToken = new MailQueueEntityToken(mailQueue.Id);
            var addNewTreeRefresher = CreateAddNewTreeRefresher(EntityToken);

            addNewTreeRefresher.PostRefreshMesseges(newQueueEntityToken);

            var editWorkflowAttribute = type.GetCustomAttribute<EditWorkflowAttribute>();
            var editWorkflowType = editWorkflowAttribute == null ? typeof(EditMailQueueWorkflow) : editWorkflowAttribute.EditWorkflowType;

            ExecuteWorklow(newQueueEntityToken, editWorkflowType);

        }
    }
}
