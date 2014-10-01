using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Composite.Data;

using CompositeC1Contrib.Email.Data.Types;

namespace CompositeC1Contrib.Email
{
    public class LoggingFacade
    {
        private static readonly List<Type> LogTypes = new List<Type>();

        static LoggingFacade()
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                try
                {
                    var types = asm.GetTypes()
                        .Where(t => t.IsInterface
                            && typeof(ILogItem).IsAssignableFrom(t)
                            && t.GetCustomAttribute<ImmutableTypeIdAttribute>() != null);

                    LogTypes.AddRange(types);
                }
                catch { }
            }
        }

        public static IEnumerable<ILogItem> GetLogItems(Guid messageId)
        {
            var list = new List<ILogItem>();
            foreach (var type in LogTypes)
            {
                var items = DataFacade.GetData(type).Cast<ILogItem>().Where(l => l.MailMessageId == messageId);

                list.AddRange(items);
            }

            return list.OrderBy(l => l.Timestamp);
        }
    }
}
