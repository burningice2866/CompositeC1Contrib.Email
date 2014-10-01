using System;
using System.Web.UI;

namespace CompositeC1Contrib.Email.Web.UI
{
    public abstract class BasePage : Page
    {
        protected LogViewMode View
        {
            get { return (LogViewMode)Enum.Parse(typeof(LogViewMode), Request.QueryString["view"]); }
        }

        protected string EntityToken
        {
            get { return Request.QueryString["EntityToken"]; }
        }

        protected string ConsoleId
        {
            get { return Request.QueryString["consoleId"]; }
        }

        protected string BaseUrl
        {
            get
            {
                var qs = Request.QueryString;

                return String.Format("?view={0}&queue={1}&template={2}&consoleId={3}&EntityToken={4}", View, qs["queue"], qs["template"], ConsoleId, EntityToken);
            }
        }

        protected void UpdateParents()
        {
            Util.UpdateParents(EntityToken, ConsoleId);
        }
    }
}
