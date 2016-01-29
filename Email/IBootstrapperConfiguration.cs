using System;

namespace CompositeC1Contrib.Email
{
    public interface IBootstrapperConfiguration
    {
        void HandleQueing(Action<MailEventEventArgs> handler);
        void HandleSending(Action<MailEventEventArgs> handler);
    }
}