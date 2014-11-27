using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;

using Composite.Data;

using CompositeC1Contrib.Email.Data;
using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Email.C1Console.Workflows
{
    public sealed class CreateMailQueueWorkflow : Basic1StepDialogWorkflow
    {
        public CreateMailQueueWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Email\\CreateMailQueue.xml") { }

        public static Dictionary<string, string> GetMailClientTypes()
        {
            return MailQueuesFacade.MailClientTypes.ToDictionary(t => t.AssemblyQualifiedName, t => t.Name);
        }

        public override bool Validate()
        {
            var name = GetBinding<string>("Name");

            using (var data = new DataConnection())
            {
                var nameExists = data.Get<IMailQueue>().Any(q => q.Name == name);
                if (nameExists)
                {
                    ShowFieldMessage("Name", "Mail queue with this name already exists");

                    return false;
                }
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

            using (var data = new DataConnection())
            {
                var mailQueue = data.CreateNew<IMailQueue>();

                mailQueue.Id = Guid.NewGuid();
                mailQueue.Name = name;
                mailQueue.ClientType = clientType;
                mailQueue.Port = 25;
                mailQueue.Host = "localhost";
                mailQueue.DeliveryMethod = SmtpDeliveryMethod.Network.ToString();

                mailQueue = data.Add(mailQueue);

                var newQueueEntityToken = mailQueue.GetDataEntityToken();
                var addNewTreeRefresher = CreateAddNewTreeRefresher(EntityToken);

                addNewTreeRefresher.PostRefreshMesseges(newQueueEntityToken);

                var type = MailQueuesFacade.MailClientTypes.Single(t => t.AssemblyQualifiedName == clientType);
                var editWorkflowAttribute = type.GetCustomAttribute<EditWorkflowAttribute>();
                var editWorkflowType = editWorkflowAttribute == null ? typeof(EditMailQueueWorkflow) : editWorkflowAttribute.EditWorkflowType;

                ExecuteWorklow(newQueueEntityToken, editWorkflowType);
            }
        }
    }
}
