using System.Web.Http;

using Newtonsoft.Json;

namespace CompositeC1Contrib.Email.SendGrid
{
    public class SendGridEventsProcessor
    {
        public SendGridEventsProcessor(IBootstrapperConfiguration config)
        {
            config.HttpConfiguration.Routes.MapHttpRoute("Mail SendGrid", "api/mail/sendgrid", new { controller = "SendGrid", action = "Post" });

            config.HandleSending(HandleEmailSending);
        }

        private static void HandleEmailSending(MailEventEventArgs e)
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
