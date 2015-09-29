using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Xml.Linq;

using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.Types;

using CompositeC1Contrib.Email.Events;

namespace CompositeC1Contrib.Email.GoogleAnalytics
{
    public class GoogleAnalyticsEventsProcessor : IEventsProcessor
    {
        private const string UtmQueryTemplate = "utm_source={0}&utm_medium=email&utm_campaign={1}";
        private const string BaseGaUrl = "https://www.google-analytics.com/collect?";

        private static readonly string GaCode = ConfigurationManager.AppSettings["ga:code"];

        public void HandleEmailSending(MailEventEventArgs e)
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

            using (var data = new DataConnection())
            {
                var settings = data.Get<IGoogleAnalyticsMailTemplateSettings>().SingleOrDefault(s => s.MailTemplateKey == e.TemplateKey);
                if (settings == null || !settings.Enabled)
                {
                    return;
                }

                if (!String.IsNullOrEmpty(settings.Source) && !String.IsNullOrEmpty(settings.Campaign))
                {
                    var links = doc.Body.Descendants().Where(_ => _.Name.LocalName == "a").ToList();
                    foreach (var link in links)
                    {
                        var hrefAttr = link.Attribute("href");
                        if (hrefAttr == null || String.IsNullOrEmpty(hrefAttr.Value))
                        {
                            continue;
                        }

                        if (!hrefAttr.Value.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        var uri = new Uri(hrefAttr.Value);
                        if (!IsValidHost(uri.Host))
                        {
                            continue;
                        }

                        var gaTracking = String.Format(UtmQueryTemplate, settings.Source, settings.Campaign);
                        var seperator = hrefAttr.Value.Contains("?") ? "&" : "?";

                        hrefAttr.Value = uri + seperator + gaTracking;
                    }

                }

                if (settings.TrackOpens && !String.IsNullOrEmpty(GaCode))
                {
                    var openUrl = GenerateOpenUrl(e, settings);

                    var imgElement = new XElement(Namespaces.Xhtml + "img",
                        new XAttribute("width", "1"),
                        new XAttribute("height", "1"),
                        new XAttribute("alt", String.Empty),

                        new XAttribute("src", openUrl));

                    doc.Body.Add(imgElement);
                }
            }

            e.MailMessage.Body = doc.ToString();
        }

        private static string GenerateOpenUrl(MailEventEventArgs e, IGoogleAnalyticsMailTemplateSettings settings)
        {
            var args = new Dictionary<string, string>
            {
                { "v", "1" },
                { "tid", GaCode },
                { "cid", Guid.NewGuid().ToString() },
                { "t", "event" },
                { "ec", "email" },
                { "ea", "open" },
                { "el", e.MailMessage.Subject },
                { "cm", "email" },
                { "cs", settings.Source }
            };

            return BaseGaUrl + String.Join("&", args.Select(kvp => String.Format("{0}={1}", kvp.Key, HttpUtility.UrlEncode(kvp.Value))));
        }

        private static bool IsValidHost(string host)
        {
            using (var data = new DataConnection())
            {
                var bindings = data.Get<IHostnameBinding>().ToList();
                foreach (var binding in bindings)
                {
                    if (binding.Hostname.Equals(host, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }

                    var split = binding.Aliases.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Any(s => s.Equals(host, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
