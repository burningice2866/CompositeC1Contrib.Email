using System;
using System.Linq;

using Microsoft.AspNet.SignalR;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.SignalR
{
    [C1ConsoleAuthorize]
    public class MailHub : Hub
    {
        static MailHub()
        {
            MailsFacade.Queued += MailQueued;
            MailsFacade.Sent += MailSent;
        }

        public void DeleteQueuedMessage(Guid messageId)
        {
            using (var data = new DataConnection())
            {
                var instance = data.Get<IQueuedMailMessage>().Single(m => m.Id == messageId);;

                data.Delete(instance);
            }
        }

        public void DeleteSentMessage(Guid messageId)
        {
            using (var data = new DataConnection())
            {
                var instance = data.Get<ISentMailMessage>().Single(m => m.Id == messageId); ;

                data.Delete(instance);
            }
        }

        public void UpdateQueuesCount(string consoleId)
        {
            Util.UpdateQueuesCount(consoleId);
        }

        private static void MailQueued(object sender, MailEventEventArgs e)
        {
            var m = new MailMessageModel(e.Id, e.QueueId, e.Timestamp, e.TemplateKey, e.MailMessage);
            var clients = GlobalHost.ConnectionManager.GetHubContext<MailHub>().Clients;

            clients.All.mailQueued(m);
        }

        private static void MailSent(object sender, MailEventEventArgs e)
        {
            var m = new MailMessageModel(e.Id, e.QueueId, e.Timestamp, e.TemplateKey, e.MailMessage);
            var clients = GlobalHost.ConnectionManager.GetHubContext<MailHub>().Clients;

            clients.All.mailSent(m);
        }
    }
}
