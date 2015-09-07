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
        }

        public void HandleEmailSending(MailEventEventArgs e)
        {
            var xSmtpHeader = new
            {
                unique_args = new
                {
                    mailMessageId = e.Id
                }
            };

            e.MailMessage.Headers.Add("X-SMTPAPI", JsonConvert.SerializeObject(xSmtpHeader));
        }
    }
}
