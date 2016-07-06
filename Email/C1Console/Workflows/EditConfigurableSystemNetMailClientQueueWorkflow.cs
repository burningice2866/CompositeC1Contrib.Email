using System;
using System.Collections.Generic;
using System.Net.Mail;

using Composite.C1Console.Workflow;

namespace CompositeC1Contrib.Email.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed class EditConfigurableSystemNetMailClientQueueWorkflow : EditMailQueueWorkflow
    {
        public EditConfigurableSystemNetMailClientQueueWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Email\\EditConfigurableSystemNetMailClientQueue.xml") { }

        public static IEnumerable<string> GetNetworkDeliveryOptions()
        {
            return Enum.GetNames(typeof(SmtpDeliveryMethod));
        }
    }
}
