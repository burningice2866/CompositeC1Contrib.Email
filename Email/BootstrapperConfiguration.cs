using Composite;

using CompositeC1Contrib.Email.Events;

namespace CompositeC1Contrib.Email
{
    public class BootstrapperConfiguration : IBootstrapperConfiguration
    {
        public IEventsProcessor EventsProcessor { get; private set; }

        public void UseEventsProcessor(IEventsProcessor eventsProcessor)
        {
            Verify.IsNull(EventsProcessor, "Events processor already set");

            MailsFacade.Sending += (sender, e) => eventsProcessor.HandleEmailSending(e);

            EventsProcessor = eventsProcessor;
        }
    }
}
