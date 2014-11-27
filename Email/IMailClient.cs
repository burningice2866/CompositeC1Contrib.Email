using System.Net.Mail;

namespace CompositeC1Contrib.Email
{
    public interface IMailClient
    {
        void Send(MailMessage message);
    }
}
