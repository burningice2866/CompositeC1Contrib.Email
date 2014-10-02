using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    public class MailEventsFacade
    {
        private static readonly MethodInfo CastMethod = typeof(Queryable).GetMethod("Cast", BindingFlags.Static | BindingFlags.Public);
        private static readonly MethodInfo DeleteMethod;

        private static readonly List<Type> LogTypes = new List<Type>();

        static MailEventsFacade()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(t => t.IsInterface
                            && typeof(IEvent).IsAssignableFrom(t)
                            && t.GetCustomAttribute<ImmutableTypeIdAttribute>() != null);

                    LogTypes.AddRange(types);
                }
                catch { }
            }

            var methods = typeof(DataFacade).GetMethods(BindingFlags.Static | BindingFlags.Public).Where(m => m.Name == "Delete");
            foreach (var method in methods)
            {
                var genericArguments = method.GetGenericArguments().ToList();
                if (genericArguments.Count != 1)
                {
                    continue;
                }

                var parameters = method.GetParameters().ToList();
                if (parameters.Count != 1)
                {
                    continue;
                }

                var parameter = parameters[0];
                if (!parameter.ParameterType.IsGenericType)
                {
                    continue;
                }

                var genericType = parameter.ParameterType.GetGenericTypeDefinition();
                if (genericType != typeof(IEnumerable<>))
                {
                    continue;
                }

                genericArguments = parameter.ParameterType.GetGenericArguments().ToList();
                if (genericArguments.Count != 1)
                {
                    continue;
                }

                DeleteMethod = method;

                break;
            }
        }

        public static IEnumerable<T> GetEventsOfType<T>(IMailTemplate template) where T : class, IEvent
        {
            using (var data = new DataConnection())
            {
                var messages = data.Get<ISentMailMessage>().Where(m => m.MailTemplateKey == template.Key);

                var q = from m in messages
                        join e in data.Get<T>() on m.Id equals e.MailMessageId
                        orderby e.Timestamp
                        select e;

                return q;
            }
        }

        public static IEnumerable<T> GetEventsOfType<T>(IMailMessage message) where T : class, IEvent
        {
            using (var data = new DataConnection())
            {
                return data.Get<T>().Where(e => e.MailMessageId == message.Id);
            }
        }

        public static IEnumerable<IEvent> GetEvents(IMailMessage message)
        {
            return GetEvents(message.Id);
        }

        public static IEnumerable<IEvent> GetEvents(Guid messageId)
        {
            var list = new List<IEvent>();
            foreach (var type in LogTypes)
            {
                var items = DataFacade.GetData(type).Cast<IEvent>().Where(l => l.MailMessageId == messageId);

                list.AddRange(items);
            }

            return list.OrderBy(l => l.Timestamp);
        }

        public static void ClearEvents(IMailMessage message)
        {
            foreach (var type in LogTypes)
            {
                var genericCast = CastMethod.MakeGenericMethod(type);
                var genericDelete = DeleteMethod.MakeGenericMethod(type);

                var items = DataFacade.GetData(type).Cast<IEvent>().Where(l => l.MailMessageId == message.Id);
                var converted = (IQueryable)genericCast.Invoke(null, new object[] { items });

                genericDelete.Invoke(null, new object[] { converted });
            }
        }
    }
}
