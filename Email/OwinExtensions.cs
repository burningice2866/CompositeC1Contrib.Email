using System;

using Composite;

using CompositeC1Contrib.Email.Events;

using Owin;

namespace CompositeC1Contrib.Email
{
    public static class OwinExtensions
    {
        public static void UseCompositeC1ContribEmail(this IAppBuilder app)
        {
            UseCompositeC1ContribEmail(app, null);
        }

        public static void UseCompositeC1ContribEmail(this IAppBuilder app, Action<IBootstrapperConfiguration> configurationAction)
        {
            app.MapSignalR();

            var configuration = new BootstrapperConfiguration();

            if (configurationAction != null)
            {
                configurationAction(configuration);
            }
        }
    }
}