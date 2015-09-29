using System;

using CompositeC1Contrib.Email.SendGrid;

namespace CompositeC1Contrib.Email
{
    public static class BootstrapperConfigurationExtensions
    {
        private static SendGridEventsProcessor _processor;

        public static void UseSendGrid(this IBootstrapperConfiguration config)
        {
            if (_processor != null)
            {
                throw new InvalidOperationException("Processor already initialized");
            }

            _processor = new SendGridEventsProcessor(config);
        }
    }
}
