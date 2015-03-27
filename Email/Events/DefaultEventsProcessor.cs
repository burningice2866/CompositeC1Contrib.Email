using System;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;

using Composite.Core.Xml;

namespace CompositeC1Contrib.Email.Events
{
    public class DefaultEventsProcessor : IEventsProcessor
    {
        private const string BaseApiUrl = "api/mail/event/";

        public DefaultEventsProcessor(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Routes.MapHttpRoute("Mail DefaultEvents", BaseApiUrl + "{data}", new { controller = "DefaultEvents", action = "Get" });
        }

        public static string GenerateEventUrl(Guid mailId, string email, string eventType, string link)
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
            var split = s.Split(new[] { '|' });

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

        public void HandleEmailSent(Guid id, MailMessage mailMessage)
        {
            XhtmlDocument doc;

            try
            {
                doc = XhtmlDocument.Parse(mailMessage.Body);
            }
            catch
            {
                return;
            }

            var email = mailMessage.To.First().Address;

            var openUrl = GenerateEventUrl(id, email, "open", String.Empty);
            if (!String.IsNullOrEmpty(openUrl))
            {
                var imgElement = new XElement(Namespaces.Xhtml + "img",
                    new XAttribute("width", "1"),
                    new XAttribute("height", "1"),
                    new XAttribute("alt", String.Empty),

                    new XAttribute("src", openUrl));

                doc.Body.Add(imgElement);
            }

            var links = doc.Descendants()
                .Where(f => f.Name == Namespaces.Xhtml + "a")
                .Select(f => f.Attribute("href"))
                .Where(f => f != null);

            foreach (var link in links)
            {
                var linkUrl = GenerateEventUrl(id, email, "click", link.Value);
                if (String.IsNullOrEmpty(linkUrl))
                {
                    continue;
                }

                link.Value = linkUrl;
            }

            mailMessage.Body = doc.ToString();
        }
    }
}
