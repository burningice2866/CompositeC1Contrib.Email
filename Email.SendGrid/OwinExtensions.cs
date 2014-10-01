using System.Web.Http;

using Owin;

namespace CompositeC1Contrib.Email.SendGrid
{
    public static class OwinExtensions
    {
        public static void UseCompositeC1ContribEmailSendGrid(this IAppBuilder app, HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MapHttpAttributeRoutes();
        }
    }
}
