using System.ComponentModel.Composition;
using System.Net.Mail;

namespace CompositeC1Contrib.Email
{
    [Export(typeof(IMailClient))]
    public class SystemNetMailClient : IMailClient
    {
        public void Send(MailMessage message)
        {
            using (var client = new SmtpClient())
            {
                client.Send(message);
            }
        }
    }
}
