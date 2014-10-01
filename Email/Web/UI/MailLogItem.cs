using System;
using System.Collections.Generic;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.Web.UI
{
    public class MailLogItem
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public DateTime TimeStamp { get; set; }
        public IMailTemplate Template { get; set; }

        public MailLogItem(Guid id, DateTime timeStamp, string templateKey, string subject)
        {
            using (var data = new DataConnection())
            {
                var templates = data.Get<IMailTemplate>().ToDictionary(t => t.Key);
                
                Id = id;
                Subject = subject;
                TimeStamp = timeStamp.ToLocalTime();
                Template = GetTemplateForMessage(templateKey, templates);
            }
        }

        public string FormatTimeStamp()
        {
            var now = DateTime.Now;

            if (TimeStamp.Date == now.Date)
            {
                return TimeStamp.ToString("HH:mm:ss");
            }

            if (TimeStamp.Year == now.Year)
            {
                return TimeStamp.ToString("dd-MM HH:mm:ss");
            }

            return TimeStamp.ToString("dd-MM-yyyy HH:mm:ss");
        }

        private static IMailTemplate GetTemplateForMessage(string templateKey, IDictionary<string, IMailTemplate> templates)
        {
            if (String.IsNullOrEmpty(templateKey))
            {
                return null;
            }

            if (!templates.ContainsKey(templateKey))
            {
                return null;
            }

            return templates[templateKey];
        }
    }
}
