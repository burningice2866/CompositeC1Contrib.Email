using System.Collections.Generic;

namespace CompositeC1Contrib.Email.SendGrid
{
    public static class ListExtensions
    {
        public static void AddIfNotExists(this IList<string> list, string value)
        {
            if (list.Contains(value))
            {
                return;
            }

            list.Add(value);
        }
    }
}
