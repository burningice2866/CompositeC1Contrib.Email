using System;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;

namespace CompositeC1Contrib.Email.Serialization
{
    [Serializable]
    public class SerializeableLinkedResource
    {
        private readonly string _contentId;
        private readonly Uri _contentLink;
        private readonly Stream _contentStream;
        private readonly SerializeableContentType _contentType;
        private readonly TransferEncoding _transferEncoding;

        public SerializeableLinkedResource(LinkedResource linkedResource)
        {
            _contentId = linkedResource.ContentId;
            _contentLink = linkedResource.ContentLink;
            _contentType = new SerializeableContentType(linkedResource.ContentType);
            _transferEncoding = linkedResource.TransferEncoding;

            if (linkedResource.ContentStream == null)
            {
                return;
            }

            var bytes = new byte[linkedResource.ContentStream.Length];
            linkedResource.ContentStream.Read(bytes, 0, bytes.Length);
            _contentStream = new MemoryStream(bytes);
        }

        public LinkedResource GetLinkedResource()
        {
            return new LinkedResource(_contentStream)
            {
                ContentId = _contentId,
                ContentLink = _contentLink,
                ContentType = _contentType.GetContentType(),
                TransferEncoding = _transferEncoding
            };
        }
    }
}
