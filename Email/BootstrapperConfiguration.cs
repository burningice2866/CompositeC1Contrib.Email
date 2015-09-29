using System;
using System.Web.Http;

namespace CompositeC1Contrib.Email
{
    public class BootstrapperConfiguration : IBootstrapperConfiguration
    {
        public HttpConfiguration HttpConfiguration { get; private set; }

        public BootstrapperConfiguration(HttpConfiguration httpConfiguration)
        {
            HttpConfiguration = httpConfiguration;
        }

        public void HandleQueing(Action<MailEventEventArgs> handler)
        {
            MailsFacade.Queing += (sender, e) => handler(e);
        }

        public void HandleSending(Action<MailEventEventArgs> handler)
        {
            MailsFacade.Sending += (sender, e) => handler(e);
        }
    }
}
