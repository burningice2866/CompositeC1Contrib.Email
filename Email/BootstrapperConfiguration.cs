using System;

namespace CompositeC1Contrib.Email
{
    public class BootstrapperConfiguration : IBootstrapperConfiguration
    {
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
