using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Xml.Linq;

using Composite.Core.WebClient;
using Composite.Core.WebClient.Renderings.Page;
using Composite.Core.Xml;
using Composite.Data;
using Composite.Data.Types;
using Composite.Functions;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    public abstract class MailMessageBuilder
    {
        private readonly IMailTemplate _template;
        private readonly IList<Attachment> _attachments;

        protected MailMessageBuilder(IMailTemplate template)
        {
            _template = template;
            _attachments = new List<Attachment>();
        }

        public void AddAttachment(Attachment attachment)
        {
            _attachments.Add(attachment);
        }

        public MailMessage BuildMailMessage()
        {
            var mailMessage = new MailMessage
            {
                Subject = ResolveText(_template.Subject, false),
                Body = ResolveHtml(_template.Body),
                IsBodyHtml = true
            };

            if (!String.IsNullOrEmpty(_template.From))
            {
                var resolvedFrom = ResolveText(_template.From, false);

                mailMessage.From = new MailAddress(resolvedFrom);
            }

            AppendMailAddresses(mailMessage.To, _template.To);
            AppendMailAddresses(mailMessage.CC, _template.Cc);
            AppendMailAddresses(mailMessage.Bcc, _template.Bcc);

            mailMessage.Headers.Add("X-C1Contrib-Mail-TemplateKey", _template.Key);

            foreach (var attachment in _attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }

            if (_template.EncryptMessage && !String.IsNullOrEmpty(_template.EncryptPassword))
            {
                MailsFacade.EncryptMessage(mailMessage, _template.EncryptPassword);
            }

            _attachments.Clear();

            return mailMessage;
        }

        protected string ResolveText(string text)
        {
            return ResolveText(text, true);
        }

        protected string ResolveText(string text, bool htmlEncode)
        {
            var dict = GetDictionaryFromModel();

            var builder = GetSchemaAndHost();
            if (builder != null)
            {
                dict.Add("%schema_and_host%", builder.ToString());
            }

            return dict.Aggregate(text, (current, kvp) => ReplaceText(current, kvp.Key, kvp.Value, htmlEncode));
        }

        protected string ResolveHtml(string body, FunctionContextContainer functionContextContainer, Func<string, string> resolveHtmlFunction)
        {
            body = resolveHtmlFunction(body);

            var doc = XhtmlDocument.Parse(body);

            PageRenderer.ExecuteEmbeddedFunctions(doc.Root, functionContextContainer);

            body = doc.ToString();

            body = MediaUrlHelper.ChangeInternalMediaUrlsToPublic(body);
            body = PageUrlHelper.ChangeRenderingPageUrlsToPublic(body);

            doc = XhtmlDocument.Parse(body);

            ExpandRelativePaths(doc);
            AppendHostnameToAbsolutePaths(doc);
            ResolveTextInLinks(doc);

            return doc.ToString();
        }

        protected abstract IDictionary<string, object> GetDictionaryFromModel();
        protected abstract string ResolveHtml(string body);

        private void ResolveTextInLinks(XContainer doc)
        {
            var model = GetDictionaryFromModel();

            foreach (var kvp in model)
            {
                var elements = doc.Descendants().Where(el => el.Name.LocalName == "a").ToList();
                foreach (var a in elements)
                {
                    var href = a.Attribute("href");
                    if (href == null)
                    {
                        continue;
                    }

                    href.Value = ReplaceText(href.Value, kvp.Key, kvp.Value, true);
                }
            }
        }

        private static void ExpandRelativePaths(XContainer doc)
        {
            var pathAttributes = GetPathAttributes(doc, "~/");
            if (pathAttributes.Count == 0)
            {
                return;
            }

            foreach (var attr in pathAttributes)
            {
                attr.Value = UrlUtils.ResolvePublicUrl(attr.Value);
            }
        }

        private static void AppendHostnameToAbsolutePaths(XContainer doc)
        {
            var pathAttributes = GetPathAttributes(doc, "/");
            if (pathAttributes.Count == 0)
            {
                return;
            }

            var builder = GetSchemaAndHost();
            if (builder == null)
            {
                return;
            }

            foreach (var attr in pathAttributes)
            {
                attr.Value = new Uri(builder.Uri, attr.Value).ToString();
            }
        }

        private static ICollection<XAttribute> GetPathAttributes(XContainer doc, string startsWith)
        {
            var elements = doc.Descendants().Where(f => f.Name.Namespace == Namespaces.Xhtml);
            var pathAttributes = elements.Attributes()
                .Where(f => f.Name.LocalName == "src" || f.Name.LocalName == "href" || f.Name.LocalName == "action")
                .Where(f => f.Value.StartsWith(startsWith));

            return pathAttributes.ToList();
        }

        public static UriBuilder GetSchemaAndHost()
        {
            var ctx = HttpContext.Current;
            string hostname = null;
            var scheme = "http";
            int? port = null;

            if (ctx != null)
            {
                hostname = ctx.Request.Url.Host;
                scheme = ctx.Request.Url.Scheme;
                port = ctx.Request.Url.Port;
            }
            else
            {
                using (var data = new DataConnection())
                {
                    var binding = data.Get<IHostnameBinding>().FirstOrDefault();
                    if (binding != null)
                    {
                        hostname = binding.Hostname;
                    }
                }
            }

            if (hostname == null)
            {
                return null;
            }

            return port.HasValue ? new UriBuilder(scheme, hostname, port.Value) : new UriBuilder(scheme, hostname);
        }

        private void AppendMailAddresses(ICollection<MailAddress> collection, string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return;
            }

            var split = s.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in split)
            {
                var resolvedPart = ResolveText(part, false);
                if (String.IsNullOrEmpty(resolvedPart))
                {
                    continue;
                }

                var address = new MailAddress(resolvedPart);

                collection.Add(address);
            }
        }

        private static string ReplaceText(string text, string name, object value, bool htmlEncode)
        {
            var s = (value ?? String.Empty).ToString();

            if (htmlEncode)
            {
                s = HttpUtility.HtmlEncode(s);
            }

            return text.Replace(String.Format("%{0}%", name), s);
        }
    }
}
