using System;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Email.GoogleAnalytics
{
    public class EditGoogleAnalyticsMailTemplateSettingsWorkFlow : Basic1StepDocumentWorkflow
    {
        public EditGoogleAnalyticsMailTemplateSettingsWorkFlow() : base("\\InstalledPackages\\CompositeC1Contrib.Email.GoogleAnalytics\\EditGoogleAnalyticsMailTemplateSettingsWorkFlow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Enabled"))
            {
                return;
            }

            var settings = GetSettings();

            Bindings.Add("Enabled", settings.Enabled);
            Bindings.Add("TrackOpen", settings.TrackOpen);

            Bindings.Add("UtmSource", settings.UtmSource);
            Bindings.Add("UtmTerm", settings.UtmTerm);
            Bindings.Add("UtmContent", settings.UtmContent);
            Bindings.Add("UtmCampaign", settings.UtmCampaign);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var enabled = GetBinding<bool>("Enabled");
            var trackOpen = GetBinding<bool>("TrackOpen");

            var source = GetBinding<string>("UtmSource");
            var term = GetBinding<string>("UtmTerm");
            var content = GetBinding<string>("UtmContent");
            var campaign = GetBinding<string>("UtmCampaign");

            using (var data = new DataConnection())
            {
                var settings = GetSettings();

                settings.Enabled = enabled;
                settings.UtmSource = source;
                settings.UtmTerm = term;
                settings.UtmContent = content;
                settings.UtmCampaign = campaign;
                settings.TrackOpen = trackOpen;

                data.Update(settings);
            }
        }

        private IGoogleAnalyticsMailTemplateSettings GetSettings()
        {
            var template = GetDataItemFromEntityToken<IMailTemplate>();

            using (var data = new DataConnection())
            {
                var settings = data.Get<IGoogleAnalyticsMailTemplateSettings>().SingleOrDefault(s => s.MailTemplateKey == template.Key);
                if (settings == null)
                {
                    settings = data.CreateNew<IGoogleAnalyticsMailTemplateSettings>();

                    settings.MailTemplateKey = template.Key;

                    settings = data.Add(settings);
                }

                return settings;
            }
        }
    }
}
