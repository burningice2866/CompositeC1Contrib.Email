using Newtonsoft.Json;

using Composite.Core.Application;

namespace CompositeC1Contrib.Email.SendGrid
{
    [ApplicationStartup]
    public class StartupHandler
    {
        public static void OnBeforeInitialize() { }

        public static void OnInitialized()
        {
            MailsFacade.Queing += MailsFacade_Queing;
        }

        static void MailsFacade_Queing(object sender, MailEventEventArgs e)
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
