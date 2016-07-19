using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace CompositeC1Contrib.Email.C1Console.Workflows
{
    public sealed class EditConfigurableSystemNetMailClientQueueWorkflow : EditMailQueueWorkflow
    {
        public EditConfigurableSystemNetMailClientQueueWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Email\\EditConfigurableSystemNetMailClientQueue.xml") { }

        public static IEnumerable<string> GetNetworkDeliveryOptions()
        {
            return Enum.GetNames(typeof(SmtpDeliveryMethod));
        }
    }
}
