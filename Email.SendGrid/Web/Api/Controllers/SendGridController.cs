using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using Composite.Data;

using Newtonsoft.Json;

namespace CompositeC1Contrib.Email.SendGrid.Web.Api.Controllers
{
    public class SendGridController : ApiController
    {
        [Route("api/sendgrid")]
        public IHttpActionResult Post()
        {
            var content = Request.Content.ReadAsStringAsync().Result;
            dynamic json = JsonConvert.DeserializeObject(content);

            using (var data = new DataConnection())
            {
                var dataItems = new List<ISendGridLogItem>();

                foreach (var model in json)
                {
                    var unixTime = double.Parse(model.timestamp.ToString());

                    var trackRecord = data.CreateNew<ISendGridLogItem>();

                    trackRecord.Id = Guid.NewGuid();
                    trackRecord.Email = model.email;
                    trackRecord.Timestamp = UnixTimeStampToDateTime(unixTime);
                    trackRecord.MailMessageId = Guid.Parse(model.mailMessageId.ToString());
                    trackRecord.Template = model.template;
                    trackRecord.EventName = model.@event;

                    dataItems.Add(trackRecord);
                }

                if (dataItems.Any())
                {
                    data.Add<ISendGridLogItem>(dataItems);
                }
            }

            return Ok();
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
