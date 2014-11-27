using System.Net.Mail;

namespace CompositeC1Contrib.Email
{
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
