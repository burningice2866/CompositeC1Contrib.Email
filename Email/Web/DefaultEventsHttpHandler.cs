using System;
using System.Net;
using System.Web;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.Email.Events;

namespace CompositeC1Contrib.Email.Web
{
    public class DefaultEventsHttpHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var input = (string)context.Request.RequestContext.RouteData.Values["data"];

            Tuple<Guid, string, string, string> eventData;

            if (!DefaultEventsProcessor.TryExtractEventData(input, out eventData))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                return;
            }

            using (var data = new DataConnection())
            {
                switch (eventData.Item2)
                {
                    case "open":

                        var openRecord = CreateLogItem<IEventOpen>(eventData.Item1, data);

                        openRecord.Email = eventData.Item3;

                        data.Add(openRecord);

                        break;

                    case "click":

                        var clickRecord = CreateLogItem<IEventClick>(eventData.Item1, data);

                        clickRecord.Email = eventData.Item3;
                        clickRecord.Url = eventData.Item4;

                        data.Add(clickRecord);

                        context.Response.RedirectLocation = eventData.Item4;
                        context.Response.StatusCode = (int)HttpStatusCode.Redirect;

                        break;
                }
            }
        }

        private static T CreateLogItem<T>(Guid id, DataConnection data) where T : class, IEvent
        {
            var itm = data.CreateNew<T>();

            itm.Id = Guid.NewGuid();
            itm.Timestamp = DateTime.UtcNow;
            itm.MailMessageId = id;

            return itm;
        }
    }
}
