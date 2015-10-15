using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

using Composite.Core.WebClient.UiControlLib;
using Composite.Data;

using CompositeC1Contrib.Email.C1Console;
using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.Web.UI
{
    public class LogPage : BasePage
    {
        protected Repeater rptLog;
        protected Selector ddlTemplates;
        protected Selector PageSize;

        protected ToolbarButton PrevPage;
        protected ToolbarButton NextPage;
        protected Composite.Core.WebClient.UiControlLib.TextBox PageNumber;

        protected PlaceHolder FromDateWidgetPlaceHolder;
        protected System.Web.UI.WebControls.Calendar FromDateWidget;

        protected PlaceHolder ToDateWidgetPlaceHolder;
        protected System.Web.UI.WebControls.Calendar ToDateWidget;

        protected void OnRefresh(object sender, EventArgs e)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            var from = GetFirstOccurence();

            FromDateWidget.SelectedDate = from.Date;
            FromDateWidget.VisibleDate = from;
            FromDateWidgetPlaceHolder.Visible = false;

            ToDateWidget.SelectedDate = DateTime.Now;
            ToDateWidget.VisibleDate = DateTime.Now;
            ToDateWidgetPlaceHolder.Visible = false;

            SetPageNumber(1);
        }

        protected void OnDeleteAll(object sender, EventArgs e)
        {
            using (var data = new DataConnection())
            {
                if (View == QueueFolder.Queued)
                {
                    var list = data.Get<IQueuedMailMessage>();

                    list = FilterByQueue(list);
                    list = Filter(list);

                    data.Delete<IQueuedMailMessage>(list);
                }
            }

            UpdateParents();
        }

        protected void CalendarSelectionChange(Object sender, EventArgs e)
        {
            if (sender == FromDateWidget)
            {
                FromDateWidgetPlaceHolder.Visible = false;
            }

            if (sender == ToDateWidget)
            {
                ToDateWidgetPlaceHolder.Visible = false;
            }
        }

        protected void CalendarYearClick(object sender, EventArgs e)
        {
            var btn = (LinkButton)sender;

            switch (btn.CommandName)
            {
                case "Back":

                    switch (btn.CommandArgument)
                    {
                        case "From": FromDateWidget.VisibleDate = FromDateWidget.VisibleDate.AddYears(-1); break;
                        case "To": ToDateWidget.VisibleDate = ToDateWidget.VisibleDate.AddYears(-1); break;
                    }

                    break;

                case "Forward":

                    switch (btn.CommandArgument)
                    {
                        case "From": FromDateWidget.VisibleDate = FromDateWidget.VisibleDate.AddYears(1); break;
                        case "To": ToDateWidget.VisibleDate = ToDateWidget.VisibleDate.AddYears(1); break;
                    }

                    break;
            }
        }

        protected void OnTemplateChanged(object sender, EventArgs e)
        {
            var selected = ddlTemplates.SelectedValue;
            if (selected == "[]")
            {
                selected = String.Empty;
            }

            var url = BaseUrl.Replace("template=" + Request.QueryString["template"], "template=" + selected);

            Response.Redirect("log.aspx" + url);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDefaults();

                var cmd = Request.QueryString["cmd"];
                if (!String.IsNullOrEmpty(cmd))
                {
                    var id = new Guid(Request.QueryString["id"]);

                    using (var data = new DataConnection())
                    {
                        switch (cmd)
                        {
                            case "requeue":
                                {
                                    var badMessage = data.Get<IBadMailMessage>().SingleOrDefault(m => m.Id == id);
                                    if (badMessage != null)
                                    {
                                        MailsFacade.RequeueMessageFromBadFolder(badMessage, data);
                                    }
                                }

                                break;

                            case "delete":
                                {
                                    IData instance = null;

                                    switch (View)
                                    {
                                        case QueueFolder.Queued:
                                            instance = data.Get<IQueuedMailMessage>().Single(m => m.Id == id);
                                            break;

                                        case QueueFolder.BadMail:
                                            instance = data.Get<IBadMailMessage>().Single(m => m.Id == id);
                                            break;
                                    }

                                    if (instance != null)
                                    {
                                        data.Delete(instance);
                                    }
                                }

                                break;
                        }
                    }

                    UpdateParents();
                }
            }

            string eventTarget = Request.Form["__EVENTTARGET"];
            switch (eventTarget)
            {
                case "ToDateSelectorInput":
                    ToDateWidgetPlaceHolder.Visible = !ToDateWidgetPlaceHolder.Visible;
                    FromDateWidgetPlaceHolder.Visible = false;

                    break;

                case "FromDateSelectorInput":
                    FromDateWidgetPlaceHolder.Visible = !FromDateWidgetPlaceHolder.Visible;
                    ToDateWidgetPlaceHolder.Visible = false;

                    break;
            }

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            BindControls();

            base.OnPreRender(e);
        }

        protected void Next(object sender, EventArgs e)
        {
            var pageNumber = int.Parse(PageNumber.Text);

            SetPageNumber(pageNumber + 1);
        }

        protected void Prev(object sender, EventArgs e)
        {
            var pageNumber = int.Parse(PageNumber.Text);
            if (pageNumber > 1)
            {
                SetPageNumber(pageNumber - 1);
            }
        }

        private void SetPageNumber(int pageNumber)
        {
            PageNumber.Text = pageNumber.ToString(CultureInfo.InvariantCulture);
        }

        private void BindControls()
        {
            var pageNumber = int.Parse(PageNumber.Text);
            var pageSize = int.Parse(PageSize.SelectedValue);

            var from = FromDateWidget.SelectedDate;
            var to = ToDateWidget.SelectedDate;

            using (var data = new DataConnection())
            {
                var query = GetQuery(data);

                query = FilterByQueue(query);

                var distinctTemplateIds = query
                    .Where(t => t.MailTemplateKey != null)
                    .Select(t => t.MailTemplateKey)
                    .Distinct();

                ddlTemplates.Items.Clear();
                ddlTemplates.Items.Add(new ListItem("no filter", "[]"));

                foreach (var key in distinctTemplateIds)
                {
                    var template = data.Get<IMailTemplate>().SingleOrDefault(t => t.Key == key);
                    if (template == null)
                    {
                        continue;
                    }

                    var itm = new ListItem(key, key);
                    if (itm.Value == Request.QueryString["template"])
                    {
                        itm.Selected = true;
                    }

                    ddlTemplates.Items.Add(itm);
                }

                var count = Filter(query, int.MaxValue, 1, from, to).Count();

                query = Filter(query, pageSize, pageNumber, from, to);

                PrevPage.Attributes["client_isdisabled"] = (pageNumber == 1).ToString().ToLower();
                NextPage.Attributes["client_isdisabled"] = (count <= (pageSize * pageNumber)).ToString().ToLower();

                rptLog.DataSource = SelectLogItems(query);
            }

            DataBind();
        }

        private IQueryable<IMailMessage> GetQuery(DataConnection data)
        {
            switch (View)
            {
                case QueueFolder.Queued: return data.Get<IQueuedMailMessage>();
                case QueueFolder.Sent: return data.Get<ISentMailMessage>();
                case QueueFolder.BadMail: return data.Get<IBadMailMessage>();
            }

            throw new InvalidOperationException("Invalid view type");
        }

        private DateTime GetFirstOccurence()
        {
            using (var data = new DataConnection())
            {
                var query = GetQuery(data);

                query = FilterByQueue(query);

                var msg = query.OrderBy(m => m.TimeStamp).FirstOrDefault();

                return msg == null ? DateTime.Now : msg.TimeStamp;
            }
        }

        private IQueryable<T> FilterByQueue<T>(IQueryable<T> mailMessages) where T : IMailMessage
        {
            Guid queueId;
            if (Guid.TryParse(Request.QueryString["queue"], out queueId))
            {
                mailMessages = mailMessages.Where(m => m.QueueId == queueId);
            }

            return mailMessages.OrderByDescending(m => m.TimeStamp);
        }

        private IQueryable<T> Filter<T>(IQueryable<T> mailMessages) where T : IMailMessage
        {
            var template = Request.QueryString["template"];
            if (!String.IsNullOrEmpty(template))
            {
                mailMessages = mailMessages.Where(m => m.MailTemplateKey == template);
            }

            return mailMessages;
        }

        private IQueryable<T> Filter<T>(IQueryable<T> mailMessages, int pageSize, int pageNumber, DateTime from, DateTime to) where T : IMailMessage
        {
            var template = Request.QueryString["template"];
            if (!String.IsNullOrEmpty(template))
            {
                mailMessages = mailMessages.Where(m => m.MailTemplateKey == template);
            }

            var skip = (pageNumber - 1) * pageSize;

            return mailMessages
                .Where(m => m.TimeStamp.Date >= from.Date && m.TimeStamp.Date <= to.Date)
                .Skip(skip).Take(pageSize);
        }

        private static IEnumerable<MailLogItem> SelectLogItems(IQueryable<IMailMessage> mailMessages)
        {
            return mailMessages
                .Select(m => new MailLogItem(m.Id, m.TimeStamp, m.MailTemplateKey, m.Subject))
                .ToList();
        }
    }
}
