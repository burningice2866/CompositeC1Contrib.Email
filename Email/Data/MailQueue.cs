using System;
using System.Linq;
using System.Xml.Linq;

using Composite;

namespace CompositeC1Contrib.Email.Data
{
    public class MailQueue
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string From { get; set; }
        public bool Paused { get; set; }
        public IMailClient Client { get; set; }

        public static MailQueue Load(XElement element)
        {
            var queue = new MailQueue
            {
                Id = Guid.Parse(element.Attribute("id").Value),
                Name = element.Attribute("name").Value,
                From = element.Attribute("from").Value,
                Paused = bool.Parse(element.Attribute("paused").Value)
            };

            var clientElement = element.Element("client");
            if (clientElement != null)
            {
                queue.Client = DeserializeClient(clientElement);
            }

            return queue;
        }

        public void Save()
        {
            var queues = MailQueuesFacade.GetFile();

            var queue = queues.Descendants("queue").SingleOrDefault(el => (string)el.Attribute("id") == Id.ToString());
            if (queue != null)
            {
                queue.Remove();
            }

            queue = new XElement("queue",
                new XAttribute("id", Id),
                new XAttribute("name", Name),
                new XAttribute("from", From ?? String.Empty),
                new XAttribute("paused", Paused));

            if (Client != null)
            {
                var client = SerializeClient(Client);

                queue.Add(client);
            }

            queues.Add(queue);

            MailQueuesFacade.Save(queues);
        }

        public void Delete()
        {
            var queues = MailQueuesFacade.GetFile();
            var queue = queues.Descendants("queue").Single(el => (string)el.Attribute("id") == Id.ToString());

            queue.Remove();

            MailQueuesFacade.Save(queues);
        }

        private static XElement SerializeClient(IMailClient client)
        {
            Verify.ArgumentNotNull(client, "client");

            var type = client.GetType();
            var el = new XElement("client", new XAttribute("type", type.AssemblyQualifiedName));

            foreach (var prop in type.GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var value = prop.GetValue(client);
                if (value != null)
                {
                    el.Add(new XAttribute(prop.Name, value));
                }
            }

            return el;
        }

        private static IMailClient DeserializeClient(XElement element)
        {
            Verify.ArgumentNotNull(element, "element");

            var type = Type.GetType(element.Attribute("type").Value);
            if (type == null)
            {
                return null;
            }

            var client = (IMailClient)Activator.CreateInstance(type);

            foreach (var prop in type.GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var attr = element.Attribute(prop.Name);
                if (attr == null)
                {
                    continue;
                }

                var value = Convert.ChangeType(attr.Value, prop.PropertyType);

                prop.SetValue(client, value);
            }

            return client;
        }
    }
}
