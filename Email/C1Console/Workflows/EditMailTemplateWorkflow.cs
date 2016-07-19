using System;

using Composite.C1Console.Users;
using Composite.C1Console.Workflow;
using Composite.Data;

using CompositeC1Contrib.Email.Data;
using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Email.C1Console.Workflows
{
    [AllowPersistingWorkflow(WorkflowPersistingType.Idle)]
    public sealed class EditMailTemplateWorkflow : Basic1StepDocumentWorkflow
    {
        public EditMailTemplateWorkflow() : base("\\InstalledPackages\\CompositeC1Contrib.Email\\EditMailTemplate.xml") { }

        private IMailTemplate MailTemplate
        {
            get
            {
                var dataToken = (DataEntityToken)EntityToken;

                return (IMailTemplate)dataToken.Data;
            }
        }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Key"))
            {
                return;
            }

            var template = MailTemplate;
            var content = template.GetContent(UserSettings.ActiveLocaleCultureInfo);

            Bindings.Add("Key", template.Key);

            Bindings.Add("From", template.From);
            Bindings.Add("To", template.To);
            Bindings.Add("Cc", template.Cc);
            Bindings.Add("Bcc", template.Bcc);

            Bindings.Add("LocalizedFrom", content.From);
            Bindings.Add("LocalizedTo", content.To);
            Bindings.Add("LocalizedCc", content.Cc);
            Bindings.Add("LocalizedBcc", content.Bcc);

            Bindings.Add("Subject", content.Subject);
            Bindings.Add("Body", content.Body);

            Bindings.Add("EncryptMessage", template.EncryptMessage);
            Bindings.Add("EncryptPassword", template.EncryptPassword);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var from = GetBinding<string>("From");
            var to = GetBinding<string>("To");
            var cc = GetBinding<string>("Cc");
            var bcc = GetBinding<string>("Bcc");

            var localizedFrom = GetBinding<string>("LocalizedFrom");
            var localizedTo = GetBinding<string>("LocalizedTo");
            var localizedCc = GetBinding<string>("LocalizedCc");
            var localizedBcc = GetBinding<string>("LocalizedBcc");

            var subject = GetBinding<string>("Subject");
            var body = GetBinding<string>("Body");

            var encryptMessage = GetBinding<bool>("EncryptMessage");
            var encryptPassword = GetBinding<string>("EncryptPassword");

            using (var data = new DataConnection())
            {
                var template = MailTemplate;
                var content = template.GetContent(UserSettings.ActiveLocaleCultureInfo);

                template.From = from;
                template.To = to;
                template.Cc = cc;
                template.Bcc = bcc;

                content.From = localizedFrom;
                content.To = localizedTo;
                content.Cc = localizedCc;
                content.Bcc = localizedBcc;

                content.Subject = subject;
                content.Body = body;

                template.EncryptMessage = encryptMessage;
                template.EncryptPassword = encryptPassword;

                data.Update(template);
                data.Update(content);
            }

            SetSaveStatus(true);
        }
    }
}
