using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Email.C1Console.Workflows
{
    public sealed class CreateMailQueueWorkflow : Basic1StepDialogWorkflow
    {
        public CreateMailQueueWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Email\\CreateMailQueue.xml") { }

        public static IEnumerable<string> GetNetworkDeliveryOptions()
        {
            return Enum.GetNames(typeof(SmtpDeliveryMethod));
        }

        public override bool Validate()
        {
            var mailQueue = GetBinding<IMailQueue>("MailQueue");

            using (var data = new DataConnection())
            {
                var nameExists = data.Get<IMailQueue>().Any(q => q.Name == mailQueue.Name);

                if (nameExists)
                {
                    ShowFieldMessage("MailQueue.Name", "Mail queue with this name already exists");

                    return false;
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

            using (var data = new DataConnection())
            {
                var mailQueue = data.CreateNew<IMailQueue>();

                mailQueue.Id = Guid.NewGuid();
                mailQueue.Port = 25;
                mailQueue.Host = "localhost";
                mailQueue.DeliveryMethod = SmtpDeliveryMethod.Network.ToString();

                Bindings.Add("MailQueue", mailQueue);
            }
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var mailQueue = GetBinding<IMailQueue>("MailQueue");

            using (var data = new DataConnection())
            {
                mailQueue = data.Add(mailQueue);
            }

            var newQueueEntityToken = mailQueue.GetDataEntityToken();
            var addNewTreeRefresher = CreateAddNewTreeRefresher(EntityToken);

            addNewTreeRefresher.PostRefreshMesseges(newQueueEntityToken);

            ExecuteWorklow(newQueueEntityToken, typeof(EditMailQueueWorkflow));
        }
    }
}
