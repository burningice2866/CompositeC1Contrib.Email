using System;
using System.Net.Mail;

namespace CompositeC1Contrib.Email.Events
{
    public interface IEventsProcessor
    {
        void HandleEmailSent(Guid id, MailMessage mail);
    }
}
