using System;
using System.Linq;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.Workflows;

namespace CompositeC1Contrib.Email.GoogleAnalytics
{
    public class EditGoogleAnalyticsMailTemplateSettingsWorkFlow : Basic1StepDialogWorkflow
    {
        public EditGoogleAnalyticsMailTemplateSettingsWorkFlow() : base("\\InstalledPackages\\CompositeC1Contrib.Email\\EditGoogleAnalyticsMailTemplateSettingsWorkFlow.xml") { }

        public override void OnInitialize(object sender, EventArgs e)
        {
            if (BindingExist("Enabled"))
            {
                return;
            }

            var settings = GetSettings();

            Bindings.Add("Enabled", settings.Enabled);
            Bindings.Add("Source", settings.Source);
            Bindings.Add("Campaign", settings.Campaign);
            Bindings.Add("TrackOpens", settings.TrackOpens);
        }

        public override void OnFinish(object sender, EventArgs e)
        {
            var enabled = GetBinding<bool>("Enabled");
            var source = GetBinding<string>("Source");
            var campaign = GetBinding<string>("Campaign");
            var trackOpens = GetBinding<bool>("TrackOpens");

            using (var data = new DataConnection())
            {
                var settings = GetSettings();

                settings.Enabled = enabled;
                settings.Source = source;
                settings.Campaign = campaign;
                settings.TrackOpens = trackOpens;

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
