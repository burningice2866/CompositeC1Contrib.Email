using System.Security.Principal;

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

using Composite.C1Console.Security;

namespace CompositeC1Contrib.Email.SignalR
{
    public class C1ConsoleAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool UserAuthorized(IPrincipal user)
        {
            return UserValidationFacade.IsLoggedIn() && base.UserAuthorized(user);
        }

        public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
        {
            return UserValidationFacade.IsLoggedIn() && base.AuthorizeHubConnection(hubDescriptor, request);
        }

        public override bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            return UserValidationFacade.IsLoggedIn() && base.AuthorizeHubMethodInvocation(hubIncomingInvokerContext, appliesToMethod);
        }
    }
}
