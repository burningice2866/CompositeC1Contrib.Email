using System;
using System.Net.Mail;
using System.Web.Http;

using CompositeC1Contrib.Email.Events;

using Newtonsoft.Json;

namespace CompositeC1Contrib.Email.SendGrid
{
    public class SendGridEventProcessor : IEventsProcessor
    {
        public SendGridEventProcessor(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Routes.MapHttpRoute("Mail SendGrid", "api/mail/sendgrid", new { controller = "SendGrid", action = "Post" });
            httpConfiguration.Routes.MapHttpRoute("Mail SendGrid (legacy)", "api/sendgrid", new { controller = "SendGrid", action = "Post" });
        }

        public void HandleEmailSent(Guid id, MailMessage mail)
        {
            var xSmtpHeader = new
            {
                unique_args = new
                {
                    mailMessageId = id
                }
            };

            mail.Headers.Add("X-SMTPAPI", JsonConvert.SerializeObject(xSmtpHeader));
        }
    }
}
