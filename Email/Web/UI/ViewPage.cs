using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

using Composite.C1Console.Events;
using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email.Web.UI
{
    public class ViewPage : BasePage
    {
        protected Repeater rptAttachments;

        protected DateTime TimeStamp { get; private set; }
        protected MailMessage Message { get; private set; }
        protected string Body;

        protected Guid Id
        {
            get
            {
                Guid id;
                Guid.TryParse(Request.QueryString["id"], out id);

                return id;
            }
        }

        protected void OnLog(object sender, EventArgs e)
        {
            Response.Redirect("messageLog.aspx?id=" + Id);
        }

        protected void OnBack(object sender, EventArgs e)
        {
            GoBack();
        }

        protected void OnDelete(object sender, EventArgs e)
        {
            using (var data = new DataConnection())
            {
                IData instance = null;

                switch (View)
                {
                    case LogViewMode.Queued: instance = data.Get<IQueuedMailMessage>().Single(m => m.Id == Id); break;
                    case LogViewMode.Sent: instance = data.Get<ISentMailMessage>().Single(m => m.Id == Id); break;
                }

                data.Delete(instance);
            }

            UpdateParents();
            GoBack();
        }

        protected void OnDownload(object sender, EventArgs e)
        {
            var url = "/Composite/InstalledPackages/CompositeC1Contrib.Email/view.aspx?view=" + View + "&id=" + Id + "&cmd=download";
            var queueItem = new DownloadFileMessageQueueItem(url);

            ConsoleMessageQueueFacade.Enqueue(queueItem, ConsoleId);
        }

        protected override void OnLoad(EventArgs e)
        {
            switch (View)
            {
                case LogViewMode.Queued: GetQueuedMailMessage(Id); break;
                case LogViewMode.Sent: GetSentMailMessage(Id); break;
            }

            if (String.IsNullOrEmpty(Body))
            {
                var view = Message.AlternateViews.SingleOrDefault(v => v.ContentType.MediaType == "text/html");
                if (view != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        view.ContentStream.CopyTo(ms);

                        var bytes = ms.ToArray();

                        Body = Encoding.UTF8.GetString(bytes);
                    }
                }
            }

            if (!IsPostBack)
            {
                if (Request.QueryString["cmd"] == "download")
                {
                    Response.Clear();

                    var attachmentId = Request.QueryString["attachmentId"];
                    if (attachmentId != null)
                    {
                        var attachment = Message.Attachments.Single(a => a.ContentId == attachmentId);

                        Response.AddHeader("Content-Disposition", "attachment; filename=" + attachment.Name);
                        Response.AddHeader("Content-Type", attachment.ContentType.MediaType);
                        attachment.ContentStream.CopyTo(Response.OutputStream);
                    }
                    else
                    {
                        var eml = MailMessageSerializeFacade.ToEml(Message);

                        Response.AddHeader("Content-Disposition", "attachment; filename=" + Id + ".eml");
                        Response.AddHeader("Content-Type", "message/rfc822");
                        Response.Write(eml);
                    }

                    Response.End();
                }
                else
                {
                    if (Message.Attachments.Any())
                    {
                        var list = Message.Attachments.Select(attachment => new MailAttachmentItem()
                        {
                            Id = attachment.ContentId,
                            Name = attachment.Name,
                            Size = attachment.ContentStream.Length
                        });

                        rptAttachments.DataSource = list;
                        rptAttachments.DataBind();
                    }
                }
            }

            base.OnLoad(e);
        }

        private void GoBack()
        {
            Response.Redirect("log.aspx" + BaseUrl);
        }

        private void GetQueuedMailMessage(Guid id)
        {
            using (var data = new DataConnection())
            {
                var instance = data.Get<IQueuedMailMessage>().Single(m => m.Id == id);

                TimeStamp = instance.TimeStamp.ToLocalTime();
                Message = MailMessageSerializeFacade.DeserializeFromBase64(instance.SerializedMessage);
                Body = Message.IsBodyHtml ? Message.Body : HtmlEncode(Message.Body);
            }
        }

        private void GetSentMailMessage(Guid id)
        {
            using (var data = new DataConnection())
            {
                var instance = data.Get<ISentMailMessage>().Single(m => m.Id == id);

                TimeStamp = instance.TimeStamp.ToLocalTime();
                Message = MailMessageSerializeFacade.ReadMailMessageFromDisk(id);
                Body = Message.IsBodyHtml ? Message.Body : HtmlEncode(Message.Body);
            }
        }

        private static string HtmlEncode(string text)
        {
            return HttpUtility.HtmlEncode(text).Replace("\r", String.Empty).Replace("\n", "<br />");
        }
    }
}
