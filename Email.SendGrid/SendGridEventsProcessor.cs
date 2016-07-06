using System;
using System.Web.Routing;

using CompositeC1Contrib.Email.SendGrid.Web;
using CompositeC1Contrib.Web;

namespace CompositeC1Contrib.Email.SendGrid
{
    public class SendGridEventsProcessor
    {
        public SendGridEventsProcessor(IBootstrapperConfiguration config, Action<MailEventEventArgs, SmtpApi> smtpApiConfigurator)
        {
            RouteTable.Routes.AddGenericHandler<SendGridHttpHandler>("api/mail/sendgrid");

            config.HandleQueing(e =>
            {
                var smtpApi = SmtpApi.FromMessage(e.MailMessage);

                smtpApi.UniqueArgs.Add("mailMessageId", e.Id.ToString());

                if (!String.IsNullOrEmpty(e.TemplateKey))
                {
                    smtpApi.Categories.AddIfNotExists(e.TemplateKey);
                }

                if (smtpApiConfigurator != null)
                {
                    smtpApiConfigurator(e, smtpApi);
                }

                smtpApi.AddToMessage(e.MailMessage);
            });
        }
    }
}
