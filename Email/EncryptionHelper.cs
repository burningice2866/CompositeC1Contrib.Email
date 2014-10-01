using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;

using ICSharpCode.SharpZipLib.Zip;

namespace CompositeC1Contrib.Email
{
    public class EncryptionHelper
    {
        public static void EncryptMessage(MailMessage mailMessage, string password)
        {
            var byteArray = Encoding.Default.GetBytes(mailMessage.Body);

            var fileStreamProviders = new Dictionary<string, Func<Stream>>
			{
				{"content.html", () => new MemoryStream((byteArray))}
			};

            for (int i = 0; i < mailMessage.Attachments.Count; i++)
            {
                var file = mailMessage.Attachments[i];

                fileStreamProviders.Add("Attached file (" + (i + 1) + ") " + file.Name, () => file.ContentStream);
            }

            var encryptedAttachment = GetPasswordProtectedZip(fileStreamProviders, password);

            mailMessage.Attachments.Clear();
            mailMessage.Attachments.Add(encryptedAttachment);

            mailMessage.Body = "Encrypted message attached";
            mailMessage.IsBodyHtml = false;
        }

        private static Attachment GetPasswordProtectedZip(Dictionary<string, Func<Stream>> fileStreamProviders, string password)
        {
            var memStream = new MemoryStream();

            var zipOutput = new ZipOutputStream(memStream)
            {
                Password = password
            };

            zipOutput.SetLevel(3);

            foreach (var fileStreamProvider in fileStreamProviders)
            {
                var filename = fileStreamProvider.Key;
                var streamProvider = fileStreamProvider.Value;

                var zipEntry = new ZipEntry(filename)
                {
                    DateTime = DateTime.Now
                };

                zipOutput.PutNextEntry(zipEntry);

                using (var streamReader = streamProvider())
                {
                    streamReader.CopyTo(zipOutput, 4096);
                }

                zipOutput.CloseEntry();
            }

            zipOutput.IsStreamOwner = false;
            zipOutput.Close();
            memStream.Position = 0;

            return new Attachment(memStream, "EncryptedMail.zip", "application/zip");
        }
    }
}
