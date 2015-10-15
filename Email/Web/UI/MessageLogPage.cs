using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI.WebControls;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;
using CompositeC1Contrib.Email.Events;

namespace CompositeC1Contrib.Email.Web.UI
{
    public class MessageLogPage : BasePage
    {
        protected Repeater rptLogColumns;
        protected Repeater rptLog;

        private IList<string> _columns;

        protected Guid Id
        {
            get
            {
                Guid id;
                Guid.TryParse(Request.QueryString["id"], out id);

                return id;
            }
        }

        protected void OnBack(object sender, EventArgs e)
        {
            GoBack();
        }

        protected void OnRefresh(object sender, EventArgs e)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            var logItems = MailEventsFacade.GetEvents(Id);
            _columns = ResolveFields(logItems);

            rptLogColumns.DataSource = _columns;
            rptLogColumns.DataBind();

            rptLog.DataSource = logItems;
            rptLog.DataBind();

            base.OnLoad(e);
        }

        protected IEnumerable<string> GetColumnsForLogItems(IEvent logItem)
        {
            var type = logItem.GetType();
            var list = new List<string>();

            foreach (var c in _columns)
            {
                if (c == "Event")
                {
                    if (logItem is IEventError)
                    {
                        list.Add("error");

                        continue;
                    }

                    if (logItem is IEventOpen)
                    {
                        list.Add("open");

                        continue;
                    }

                    if (logItem is IEventClick)
                    {
                        list.Add("click");

                        continue;
                    }
                }

                var prop = type.GetProperty(c);
                if (prop == null)
                {
                    list.Add(String.Empty);

                    continue;
                }

                var value = prop.GetValue(logItem);
                if (value == null)
                {
                    list.Add(String.Empty);

                    continue;
                }

                if (value is DateTime)
                {
                    value = ((DateTime)value).ToString("G");
                }

                list.Add(value.ToString());
            }

            return list;
        }

        private void GoBack()
        {
            Response.Redirect("view.aspx" + BaseUrl + "&id=" + Id);
        }

        private static IList<string> ResolveFields(IEnumerable<IEvent> logItems)
        {
            var columns = new List<string>();
            var resolvedTypes = new HashSet<Type>();

            foreach (var itm in logItems)
            {
                var type = itm.DataSourceId.InterfaceType;
                if (resolvedTypes.Contains(type))
                {
                    continue;
                }

                var fields = GetPropertiesRecursive(type);
                foreach (var name in fields.Select(f => f.Name).Where(name => !columns.Contains(name)))
                {
                    columns.Add(name);
                }

                resolvedTypes.Add(type);
            }

            return columns;
        }

        private static IEnumerable<PropertyInfo> GetPropertiesRecursive(Type t)
        {
            var interfaces = t.GetInterfaces().ToList();

            interfaces.Add(t);

            return (from i in interfaces
                    where i != typeof(IData)
                    from f in i.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty)
                    let name = f.Name
                    where name != "Id" && name != "MailMessageId"
                    select f).ToList();
        }
    }
}
