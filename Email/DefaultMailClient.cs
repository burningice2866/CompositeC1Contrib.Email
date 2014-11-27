using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Hosting;

using Composite;
using Composite.Core;

using CompositeC1Contrib.Email.C1Console.Workflows;
using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    [EditWorkflow(typeof(EditDefaultMailClientQueueWorkflow))]
    public class DefaultMailClient : IMailClient
    {
        private readonly SmtpClient _client;

        public DefaultMailClient(IMailQueue queue)
        {
            _client = ResolveSmtpClientClient(queue);
        }

        public void Send(MailMessage message)
        {
            _client.Send(message);
        }

        private static SmtpClient ResolveSmtpClientClient(IMailQueue queue)
        {
            try
            {
                var smtpClient = new SmtpClient
                {
                    DeliveryMethod = (SmtpDeliveryMethod)Enum.Parse(typeof(SmtpDeliveryMethod), queue.DeliveryMethod),
                    Host = queue.Host,
                    Port = queue.Port,
                    EnableSsl = queue.EnableSsl,
                    TargetName = queue.TargetName
                };

                if (smtpClient.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
                {
                    Verify.StringNotIsNullOrWhiteSpace(queue.PickupDirectoryLocation);

                    var pickupDirectoryLocation = queue.PickupDirectoryLocation;

                    if (pickupDirectoryLocation.StartsWith("/") || pickupDirectoryLocation.StartsWith("~/"))
                    {
                        pickupDirectoryLocation = HostingEnvironment.MapPath(pickupDirectoryLocation);
                    }

                    if (!Directory.Exists(pickupDirectoryLocation))
                    {
                        Directory.CreateDirectory(pickupDirectoryLocation);
                    }

                    smtpClient.PickupDirectoryLocation = pickupDirectoryLocation;
                }

                if (queue.DefaultCredentials)
                {
                    smtpClient.Credentials = (NetworkCredential)CredentialCache.DefaultCredentials;
                }
                else if (!String.IsNullOrEmpty(queue.UserName) && !String.IsNullOrEmpty(queue.Password))
                {
                    smtpClient.Credentials = new NetworkCredential(queue.UserName, queue.Password);
                }

                return smtpClient;
            }
            catch (Exception exc)
            {
                Log.LogCritical("Invalid smtp settings", exc);

                return null;
            }
        }
    }
}
