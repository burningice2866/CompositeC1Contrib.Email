using System;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Hosting;

using CompositeC1Contrib.Email.Serialization;

namespace CompositeC1Contrib.Email
{
    public class MailMessageSerializeFacade
    {
        private static readonly string BasePath = HostingEnvironment.MapPath("~/App_Data/MailMessage/");

        static MailMessageSerializeFacade()
        {
            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }
        }

        public static MailMessage ReadMailMessageFromDisk(Guid id)
        {
            var path = Path.Combine(BasePath, id + ".bin");
            using (var fs = File.Open(path, FileMode.Open))
            {
                var serializedMailMessage = (SerializeableMailMessage)new BinaryFormatter().Deserialize(fs);

                return serializedMailMessage.GetMailMessage();
            }
        }

        public static void SaveMailMessageToDisk(Guid id, MailMessage mailMessage)
        {
            var serializedMailMessage = new SerializeableMailMessage(mailMessage);

            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, serializedMailMessage);

                using (var fs = File.Create(Path.Combine(BasePath, id + ".bin")))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.CopyTo(fs);
                }
            }
        }

        public static string SerializeAsBase64(MailMessage mailMessage)
        {
            using (var ms = new MemoryStream())
            {
                var serializedMailMessage = new SerializeableMailMessage(mailMessage);

                new BinaryFormatter().Serialize(ms, serializedMailMessage);

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static MailMessage DeserializeFromBase64(string serializedMessage)
        {
            byte[] bytes = Convert.FromBase64String(serializedMessage);

            using (var ms = new MemoryStream(bytes))
            {
                var serializedMailMessage = (SerializeableMailMessage)new BinaryFormatter().Deserialize(ms);

                return serializedMailMessage.GetMailMessage();
            }
        }

        public static string ToEml(MailMessage message)
        {
            var assembly = typeof(SmtpClient).Assembly;
            var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

            using (var memoryStream = new MemoryStream())
            {
                // Get reflection info for MailWriter contructor
                var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

                // Construct MailWriter object with our FileStream
                var mailWriter = mailWriterContructor.Invoke(new object[] { memoryStream });

                // Get reflection info for Send() method on MailMessage
                var sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

                // Call method passing in MailWriter
                sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { mailWriter, true, true }, null);

                // Finally get reflection info for Close() method on our MailWriter
                var closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

                // Call close method
                closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);

                return Encoding.ASCII.GetString(memoryStream.ToArray());
            }
        }
    }
}
