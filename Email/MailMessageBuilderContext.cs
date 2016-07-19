using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Composite;
using Composite.Core.Caching;
using Composite.Data;
using Composite.Data.Types;

namespace CompositeC1Contrib.Email
{
    public class MailMessageBuilderContext : IDisposable
    {
        private static MethodInfo GetCachedOrNewMethod = typeof(RequestLifetimeCache).GetMethod("GetCachedOrNew", BindingFlags.Static | BindingFlags.NonPublic);
        private static MethodInfo GetCachedOrNewMethod_Generic = GetCachedOrNewMethod.MakeGenericMethod(typeof(Stack<MailMessageBuilderContext>));

        public Guid WebsiteId { get; private set; }
        public CultureInfo Culture { get; private set; }

        public static MailMessageBuilderContext Current
        {
            get
            {
                var currentStack = Stack;

                return currentStack.Any() ? currentStack.Peek() : null;
            }
        }

        private static Stack<MailMessageBuilderContext> Stack
        {
            get { return (Stack<MailMessageBuilderContext>)GetCachedOrNewMethod_Generic.Invoke(null, new[] { "MailMessageBuilderContext:Stack" }); }
        }

        public MailMessageBuilderContext() : this(CultureInfo.CurrentCulture) { }

        public MailMessageBuilderContext(CultureInfo culture)
        {
            using (var data = new DataConnection())
            {
                WebsiteId = (from s in data.Get<IPageStructure>()
                             where s.ParentId == default(Guid)
                             orderby s.LocalOrdering
                             select s.Id).First();
            }

            Culture = culture;

            Stack.Push(this);
        }

        public MailMessageBuilderContext(Guid websiteId, CultureInfo culture)
        {
            WebsiteId = websiteId;
            Culture = culture;

            Stack.Push(this);
        }

        public void Dispose()
        {
            var top = Stack.Pop();

            Verify.That(object.ReferenceEquals(top, this), "MailMessageBuilderContext weren't disposed properly");
        }
    }
}
