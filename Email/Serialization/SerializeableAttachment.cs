using System;
using System.IO;
using System.Net.Mail;
using System.Text;

namespace CompositeC1Contrib.Email.Serialization
{
    [Serializable]
    public class SerializeableAttachment
    {
        private readonly string _contentId;
        private readonly SerializeableContentDisposition _contentDisposition;
        private readonly SerializeableContentType _contentType;
        private readonly Stream _contentStream;
        private readonly System.Net.Mime.TransferEncoding _transferEncoding;
        private readonly string _name;
        private readonly Encoding _nameEncoding;

        public SerializeableAttachment(Attachment attachment)
        {
            _contentId = attachment.ContentId;
            _contentDisposition = new SerializeableContentDisposition(attachment.ContentDisposition);
            _contentType = new SerializeableContentType(attachment.ContentType);
            _name = attachment.Name;
            _transferEncoding = attachment.TransferEncoding;
            _nameEncoding = attachment.NameEncoding;

            if (attachment.ContentStream == null)
            {
                return;
            }

            byte[] bytes = new byte[attachment.ContentStream.Length];
            attachment.ContentStream.Read(bytes, 0, bytes.Length);

            _contentStream = new MemoryStream(bytes);
        }

        public Attachment GetAttachment()
        {
            var attachment = new Attachment(_contentStream, _name)
            {
                ContentId = _contentId,
                ContentType = _contentType.GetContentType(),
                Name = _name,
                TransferEncoding = _transferEncoding,
                NameEncoding = _nameEncoding,
            };

            _contentDisposition.CopyTo(attachment.ContentDisposition);

            return attachment;
        }   
    }
}
