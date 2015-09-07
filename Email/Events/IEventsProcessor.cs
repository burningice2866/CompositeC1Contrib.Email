namespace CompositeC1Contrib.Email.Events
{
    public interface IEventsProcessor
    {
        void HandleEmailSending(MailEventEventArgs e);
    }
}
