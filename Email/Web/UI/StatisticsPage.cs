using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.Web.UI
{
    public class StatisticsPage : BasePage
    {
        protected Literal litFirstSent;
        protected Literal litLastSent;
        protected Literal litSent;
        protected Literal litOpened;
        protected Literal litCLicked;

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                using (var data = new DataConnection())
                {
                    var templateKey = Request.QueryString["template"];
                    var template = data.Get<IMailTemplate>().Single(t => t.Key == templateKey);

                    var sent = MailEventsFacade.GetEventsOfType<IEventBasic>(template).Where(ev => ev.Event == "sent").ToList();

                    litSent.Text = sent.Count.ToString(CultureInfo.InvariantCulture);

                    if (sent.Count > 0)
                    {
                        var opens = MailEventsFacade.GetEventsOfType<IEventOpen>(template).Count();
                        var clicks = MailEventsFacade.GetEventsOfType<IEventClick>(template).Count();

                        litFirstSent.Text = sent.First().Timestamp.ToString("G");
                        litLastSent.Text = sent.Last().Timestamp.ToString("G");
                        litOpened.Text = opens.ToString(CultureInfo.InvariantCulture);
                        litCLicked.Text = clicks.ToString(CultureInfo.InvariantCulture);
                    }

                }
            }

            base.OnLoad(e);
        }
    }
}
