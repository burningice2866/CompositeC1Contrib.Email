using Owin;

namespace CompositeC1Contrib.Email
{
    public static class OwinExtensions
    {
        public static void UseCompositeC1ContribEmail(this IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
