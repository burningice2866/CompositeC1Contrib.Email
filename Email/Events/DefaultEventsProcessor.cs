﻿using System;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;

using Composite.Core.Xml;

namespace CompositeC1Contrib.Email.Events
{
    public class DefaultEventsProcessor
    {
        private const string BaseApiUrl = "api/mail/event/";

        private readonly DefaultEventsProcessorOptions _options;

        public DefaultEventsProcessor(IBootstrapperConfiguration config, DefaultEventsProcessorOptions options)
        {
            _options = options;

            config.HttpConfiguration.Routes.MapHttpRoute("Mail DefaultEvents", BaseApiUrl + "{data}", new { controller = "DefaultEvents", action = "Get" });

            config.HandleQueing(HandleEvent);
        }

        private void HandleEvent(MailEventEventArgs e)
        {
            XhtmlDocument doc;

            try
            {
                doc = XhtmlDocument.Parse(e.MailMessage.Body);
            }
            catch
            {
                return;
            }

            var email = e.MailMessage.To.First().Address;

            if (_options.TrackOpen)
            {

                var openUrl = GenerateEventUrl(e.Id, email, "open", String.Empty);
                if (!String.IsNullOrEmpty(openUrl))
                {
                    var imgElement = new XElement(Namespaces.Xhtml + "img",
                        new XAttribute("width", "1"),
                        new XAttribute("height", "1"),
                        new XAttribute("alt", String.Empty),

                        new XAttribute("src", openUrl));

                    doc.Body.Add(imgElement);
                }
            }

            if (_options.TrackClick)
            {
                var links = doc.Descendants()
                    .Where(f => f.Name == Namespaces.Xhtml + "a")
                    .Select(f => f.Attribute("href"))
                    .Where(f => f != null);

                foreach (var link in links)
                {
                    var linkUrl = GenerateEventUrl(e.Id, email, "click", link.Value);
                    if (String.IsNullOrEmpty(linkUrl))
                    {
                        continue;
                    }

                    link.Value = linkUrl;
                }
            }

            e.MailMessage.Body = doc.ToString();
        }

        private static string GenerateEventUrl(Guid mailId, string email, string eventType, string link)
        {
            var builder = MailMessageBuilder.GetSchemaAndHost();
            if (builder == null)
            {
                return null;
            }

            var data = String.Format("{0}|{1}|{2}|{3}", mailId, eventType, email, link);
            var bytes = Encoding.UTF8.GetBytes(data);

            var url = BaseApiUrl + Convert.ToBase64String(bytes);

            return new Uri(builder.Uri, url).ToString();
        }

        public static bool TryExtractEventData(string data, out Tuple<Guid, string, string, string> eventData)
        {
            eventData = null;

            if (String.IsNullOrEmpty(data))
            {
                return false;
            }

            var bytes = Convert.FromBase64String(data);
            var s = Encoding.UTF8.GetString(bytes);
            var split = s.Split('|');

            if (split.Length != 4)
            {
                return false;
            }

            Guid mailId;
            if (!Guid.TryParse(split[0], out mailId))
            {
                return false;
            }

            var eventType = split[1];
            var email = split[2];
            var link = split[3];

            eventData = Tuple.Create(mailId, eventType, email, link);

            return true;
        }
    }
}
