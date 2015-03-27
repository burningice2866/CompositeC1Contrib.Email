using Composite;

using CompositeC1Contrib.Email.Events;

namespace CompositeC1Contrib.Email
{
    public class BootstrapperConfiguration : IBootstrapperConfiguration
    {
        public IEventsProcessor EventsProcessor { get; private set; }

        public void UseEventsProcessor(IEventsProcessor eventProcessor)
        {
            Verify.IsNull(EventsProcessor, "EventsProcessor already set");

            MailsFacade.Sending += (sender, e) => eventProcessor.HandleEmailSent(e.Id, e.MailMessage);

            EventsProcessor = eventProcessor;
        }
    }
}
