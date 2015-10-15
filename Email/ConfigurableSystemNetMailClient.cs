using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Mail;
using System.Web.Hosting;

using Composite;
using Composite.Core;
using Composite.Core.IO;

using CompositeC1Contrib.Email.C1Console.Workflows;

namespace CompositeC1Contrib.Email
{
    [Export(typeof(IMailClient))]
    [EditWorkflow(typeof(EditConfigurableSystemNetMailClientQueueWorkflow))]
    public class ConfigurableSystemNetMailClient : IMailClient
    {
        private SmtpClient _client;

        public string DeliveryMethod { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string TargetName { get; set; }

        public string PickupDirectoryLocation { get; set; }

        public bool DefaultCredentials { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public void Send(MailMessage message)
        {
            if (_client == null)
            {
                _client = ResolveSmtpClientClient();
            }

            _client.Send(message);
        }

        private SmtpClient ResolveSmtpClientClient()
        {
            try
            {
                var smtpClient = new SmtpClient
                {
                    DeliveryMethod = (SmtpDeliveryMethod)Enum.Parse(typeof(SmtpDeliveryMethod), DeliveryMethod),
                    Host = Host,
                    Port = Port,
                    EnableSsl = EnableSsl,
                    TargetName = TargetName
                };

                if (smtpClient.DeliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
                {
                    Verify.StringNotIsNullOrWhiteSpace(PickupDirectoryLocation);

                    var pickupDirectoryLocation = PickupDirectoryLocation;

                    if (pickupDirectoryLocation.StartsWith("/") || pickupDirectoryLocation.StartsWith("~/"))
                    {
                        pickupDirectoryLocation = HostingEnvironment.MapPath(pickupDirectoryLocation);
                    }

                    if (!C1Directory.Exists(pickupDirectoryLocation))
                    {
                        C1Directory.CreateDirectory(pickupDirectoryLocation);
                    }

                    smtpClient.PickupDirectoryLocation = pickupDirectoryLocation;
                }

                if (DefaultCredentials)
                {
                    smtpClient.Credentials = (NetworkCredential)CredentialCache.DefaultCredentials;
                }
                else if (!String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password))
                {
                    smtpClient.Credentials = new NetworkCredential(UserName, Password);
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
