using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

using Newtonsoft.Json;

namespace CompositeC1Contrib.Email.SendGrid
{
    public class SmtpApi
    {
        [JsonProperty("category")]
        public List<string> Categories { get; set; }

        [JsonProperty("unique_args")]
        public Dictionary<string, string> UniqueArgs { get; set; }

        public SmtpApi()
        {
            Categories = new List<string>();
            UniqueArgs = new Dictionary<string, string>();
        }

        public static SmtpApi FromMessage(MailMessage message)
        {
            var json = message.Headers["X-SMTPAPI"];
            if (String.IsNullOrEmpty(json))
            {
                var smtpApi = new SmtpApi();

                var headers = message.Headers;

                var culture = headers["X-C1Contrib-Mail-Culture"];
                if (!String.IsNullOrEmpty(culture))
                {
                    smtpApi.UniqueArgs.Add("Culture", culture);
                }

                var queue = headers["X-C1Contrib-Mail-Queue"];
                if (!String.IsNullOrEmpty(queue))
                {
                    smtpApi.UniqueArgs.Add("Queue", queue);
                }

                return smtpApi;
            }
            else
            {
                return JsonConvert.DeserializeObject<SmtpApi>(json);
            }
        }

        public void AddToMessage(MailMessage message)
        {
            var clone = (SmtpApi)this.MemberwiseClone();

            if (!clone.Categories.Any())
            {
                clone.Categories = null;
            }

            if (!clone.UniqueArgs.Any())
            {
                clone.UniqueArgs = null;
            }

            var json = JsonConvert.SerializeObject(clone);

            message.Headers["X-SMTPAPI"] = json;
        }
    }
}
