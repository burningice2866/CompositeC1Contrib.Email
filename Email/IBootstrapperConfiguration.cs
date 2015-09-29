using System;
using System.Web.Http;

namespace CompositeC1Contrib.Email
{
    public interface IBootstrapperConfiguration
    {
        HttpConfiguration HttpConfiguration { get; }

        void HandleQueing(Action<MailEventEventArgs> handler);
        void HandleSending(Action<MailEventEventArgs> handler);
    }
}