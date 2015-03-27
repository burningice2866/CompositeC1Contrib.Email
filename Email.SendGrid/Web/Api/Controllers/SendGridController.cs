using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using Newtonsoft.Json;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.SendGrid.Web.Api.Controllers
{
    public class SendGridController : ApiController
    {
        [HttpPost]
        public IHttpActionResult Post()
        {
            var content = Request.Content.ReadAsStringAsync().Result;
            dynamic json = JsonConvert.DeserializeObject(content);

            using (var data = new DataConnection())
            {
                var openRecords = new List<IEventOpen>();
                var clickRecords = new List<IEventClick>();

                foreach (var model in json)
                {
                    var id = Guid.Empty;
                    if (model.mailMessageId == null || !Guid.TryParse(model.mailMessageId.ToString(), out id))
                    {
                        continue;
                    }

                    var e = model.@event;
                    if (e == "click")
                    {
                        var clickRecord = CreateLogItem<IEventClick>(model, id, data);

                        clickRecord.Email = model.email.ToString();
                        clickRecord.Url = model.url.ToString();

                        clickRecords.Add(clickRecord);
                    }
                    else if (e == "open")
                    {
                        var openRecord = CreateLogItem<IEventOpen>(model, id, data);

                        openRecord.Email = model.email.ToString();

                        openRecords.Add(openRecord);
                    }
                }

                if (openRecords.Any())
                {
                    data.Add<IEventOpen>(openRecords);
                }

                if (clickRecords.Any())
                {
                    data.Add<IEventClick>(clickRecords);
                }
            }

            return Ok();
        }

        private static T CreateLogItem<T>(dynamic model, Guid id, DataConnection data) where T : class, IEvent
        {
            var unixTime = double.Parse(model.timestamp.ToString());

            var itm = data.CreateNew<T>();

            itm.Id = Guid.NewGuid();
            itm.Timestamp = UnixTimeStampToDateTime(unixTime);
            itm.MailMessageId = id;

            return itm;
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);

            return dtDateTime;
        }
    }
}
