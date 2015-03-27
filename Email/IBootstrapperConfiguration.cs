using CompositeC1Contrib.Email.Events;

namespace CompositeC1Contrib.Email
{
    public interface IBootstrapperConfiguration
    {
        IEventsProcessor EventsProcessor { get; }

        void UseEventsProcessor(IEventsProcessor eventProcessor);
    }
}