using System;
using System.ComponentModel.Composition;
using System.Net.Mail;

namespace CompositeC1Contrib.Email
{
    [Export(typeof(IMailClient))]
    [Obsolete("Use 'ConfigurableSystemNetMailClient'")]
    public class DefaultMailClient : IMailClient
    {
        public void Send(MailMessage message)
        {
            throw new NotSupportedException();
        }
    }
}
