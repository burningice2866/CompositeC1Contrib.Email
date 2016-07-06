using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

using Newtonsoft.Json;

namespace CompositeC1Contrib.Email.SendGrid.Web
{
    public class SendGridHttpHandler : HttpTaskAsyncHandler
    {
        public override bool IsReusable
        {
            get { return true; }
        }

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            if (context.Request.HttpMethod != "POST")
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                return;
            }

            var content = await ReadInput(context.Request.InputStream).ConfigureAwait(false);
            dynamic json = JsonConvert.DeserializeObject(content);

            using (var data = new DataConnection())
            {
                var openRecords = new List<IEventOpen>();
                var clickRecords = new List<IEventClick>();

                foreach (var model in json)
                {
                    if (model.mailMessageId == null)
                    {
                        continue;
                    }

                    Guid id;
                    if (!Guid.TryParse(model.mailMessageId.ToString(), out id))
                    {
                        continue;
                    }

                    var e = (string)model.@event;

                    switch (e)
                    {
                        case "click":
                            {
                                var clickRecord = CreateLogItem<IEventClick>(model, id, data);

                                clickRecord.Email = model.email.ToString();
                                clickRecord.Url = model.url.ToString();

                                clickRecords.Add(clickRecord);
                            }

                            break;

                        case "open":
                            {
                                var openRecord = CreateLogItem<IEventOpen>(model, id, data);

                                openRecord.Email = model.email.ToString();

                                openRecords.Add(openRecord);
                            }

                            break;
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

        private static Task<string> ReadInput(Stream s)
        {
            using (var stream = new StreamReader(s))
            {
                return stream.ReadToEndAsync();
            }
        }
    }
}