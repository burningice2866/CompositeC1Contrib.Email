using System.Web;
using System.Web.Routing;

namespace CompositeC1Contrib.Email.Web
{
    public class GenericRouteHandler<T> : IRouteHandler where T : IHttpHandler, new()
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var handler = new T();

            return handler;
        }
    } 
}
