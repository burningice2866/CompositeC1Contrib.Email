using Newtonsoft.Json;

using Composite.Core.Application;
using Composite.Data.DynamicTypes;

namespace CompositeC1Contrib.Email.SendGrid
{
    [ApplicationStartup]
    public class StartupHandler
    {
        public static void OnBeforeInitialize() { }

        public static void OnInitialized()
        {
            DynamicTypeManager.EnsureCreateStore(typeof(ISendGridLogItem));

            MailsFacade.Queing += MailsFacade_Queing;
        }

        static void MailsFacade_Queing(object sender, MailEventEventArgs e)
        {
            var xSmtpHeader = new
            {
                unique_args = new
                {
                    template = e.TemplateKey,
                    mailMessageId = e.Id
                }
            };

            e.MailMessage.Headers.Add("X-SMTPAPI", JsonConvert.SerializeObject(xSmtpHeader));
        }
    }
}
