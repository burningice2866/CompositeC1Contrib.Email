using System;
using System.Linq;
using System.Net.Mail;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    public class MailModelsFacade
    {
        public static MailMessage BuildMailMessage(object mailModel)
        {
            using (var data = new DataConnection())
            {
                var modelType = mailModel.GetType();
                var templates = data.Get<IMailTemplate>()
                    .Where(t => t.ModelType != null && !t.ModelType.Equals(String.Empty))
                    .AsEnumerable()
                    .Where(t => Type.GetType(t.ModelType) == modelType)
                    .ToList();

                if (templates.Count == 0)
                {
                    throw new InvalidOperationException("There is no templates using the supplied model");
                }

                if (templates.Count > 1)
                {
                    throw new InvalidOperationException("There is more than one template using the supplied model");
                }

                return BuildMailMessage(templates.Single(), mailModel);
            }
        }

        public static MailMessage BuildMailMessage(string key, object mailModel)
        {
            using (var data = new DataConnection())
            {
                var template = data.Get<IMailTemplate>().SingleOrDefault(t => String.Compare(t.Key, key, StringComparison.OrdinalIgnoreCase) == 0);
                if (template == null)
                {
                    throw new ArgumentException("There is no templates with the specified key: " + key, "key");
                }

                return BuildMailMessage(template, mailModel);
            }
        }

        public static MailMessage BuildMailMessage(IMailTemplate template, object mailModel)
        {
            var builder = new ObjectModelMailMessageBuilder(template, mailModel);

            return builder.BuildMailMessage();
        }
    }
}
