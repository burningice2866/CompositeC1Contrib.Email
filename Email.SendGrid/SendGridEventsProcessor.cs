using System.Web.Routing;

using CompositeC1Contrib.Email.SendGrid.Web.Api.Controllers;
using CompositeC1Contrib.Email.Web;

using Newtonsoft.Json;

namespace CompositeC1Contrib.Email.SendGrid
{
    public class SendGridEventsProcessor
    {
        public SendGridEventsProcessor(IBootstrapperConfiguration config)
        {
            RouteTable.Routes.Add(new Route("api/mail/sendgrid}", new GenericRouteHandler<SendGridHttpHandler>()));

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
