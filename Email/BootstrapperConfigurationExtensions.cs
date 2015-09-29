using System;

using CompositeC1Contrib.Email.Events;

namespace CompositeC1Contrib.Email
{
    public static class BootstrapperConfigurationExtensions
    {
        private static DefaultEventsProcessor _processor;

        public static void UseDefaultEventsTracking(this IBootstrapperConfiguration config, DefaultEventsProcessorOptions options)
        {
            if (_processor != null)
            {
                throw new InvalidOperationException("Processor already initialized");
            }

            _processor = new DefaultEventsProcessor(config, options);
        }
    }
}
