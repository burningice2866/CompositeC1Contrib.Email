using System;

using CompositeC1Contrib.Email.GoogleAnalytics;

namespace CompositeC1Contrib.Email
{
    public static class BootstrapperConfigurationExtensions
    {
        private static GoogleAnalyticsEventsProcessor _processor;

        public static void UseGoogleAnalytics(this IBootstrapperConfiguration config, GoogleAnalyticsEventsProcessorOptions options)
        {
            if (_processor != null)
            {
                throw new InvalidOperationException("Processor already initialized");
            }

            _processor = new GoogleAnalyticsEventsProcessor(config, options);
        }
    }
}
