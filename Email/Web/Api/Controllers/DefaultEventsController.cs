using System;
using System.Web.Http;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.Email.Events;

namespace CompositeC1Contrib.Email.Web.Api.Controllers
{
    public class DefaultEventsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get(string input)
        {
            Tuple<Guid, string, string, string> eventData;

            if (!DefaultEventsProcessor.TryExtractEventData(input, out eventData))
            {
                return NotFound();
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

                        return Redirect(eventData.Item4);
                }
            }

            return Ok();
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
