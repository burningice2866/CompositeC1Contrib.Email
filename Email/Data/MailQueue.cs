using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Hosting;

using Composite;
using Composite.Core;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.Data
{
    public class MailQueue
    {
        private readonly IMailQueue _iMailQueue;

        public Guid Id
        {
            get { return _iMailQueue.Id; }
        }

        public string Name
        {
            get { return _iMailQueue.Name; }
        }

        public string From
        {
            get { return _iMailQueue.From; }
        }

        public bool Paused
        {
            get { return _iMailQueue.Paused; }
        }

        public SmtpClient SmtpClient { get; private set; }

        public MailQueue(IMailQueue iMailQueue)
        {
            _iMailQueue = iMailQueue;
            SmtpClient = ResolveSmtpClientClient(_iMailQueue);
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
