using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Composite.Core.Xml;
using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.Data
{
    public static class MailTemplateExtensions
    {
        public static IMailTemplateContent GetContent(this IMailTemplate template)
        {
            return GetContent(template, null);
        }

        public static IMailTemplateContent GetContent(this IMailTemplate template, CultureInfo culture)
        {
            var data = culture == null ? new DataConnection() : new DataConnection(culture);

            using (data)
            {
                var content = data.Get<IMailTemplateContent>().SingleOrDefault(t => t.TemplateKey == template.Key);
                if (content == null)
                {
                    content = data.CreateNew<IMailTemplateContent>();

                    content.Id = Guid.NewGuid();
                    content.TemplateKey = template.Key;
                    content.Subject = String.Empty;
                    content.Body = new XhtmlDocument().ToString();

                    content = data.Add(content);
                }

                return content;
            }
        }

        public static IMailTemplate GetTemplate(this IMailTemplateContent content)
        {
            using (var data = new DataConnection())
            {
                return data.Get<IMailTemplate>().Single(t => t.Key == content.TemplateKey);
            }
        }

        public static string From(this IMailTemplateContent content)
        {
            var template = GetTemplate(content);

            if (!String.IsNullOrEmpty(content.From))
            {
                return content.From;
            }

            return template.From;
        }

        public static IEnumerable<string> To(this IMailTemplateContent content)
        {
            var template = GetTemplate(content);

            return Join(template.To, content.To);
        }

        public static IEnumerable<string> Cc(this IMailTemplateContent content)
        {
            var template = GetTemplate(content);

            return Join(template.Cc, content.Cc);
        }

        public static IEnumerable<string> Bcc(this IMailTemplateContent content)
        {
            var template = GetTemplate(content);

            return Join(template.Bcc, content.Bcc);
        }

        private static IEnumerable<string> Join(params string[] addresses)
        {
            var list = new List<string>();

            foreach (var address in addresses)
            {
                if (!String.IsNullOrEmpty(address))
                {
                    list.AddRange(address.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }

            return list;
        }
    }
}
