using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;

namespace CompositeC1Contrib.Email.Serialization
{
    [Serializable]
    public class SerializeableAlternateView
    {
        private readonly Uri _baseUri;
        private readonly string _contentId;
        private readonly Stream _contentStream;
        private readonly SerializeableContentType _contentType;
        private readonly IList<SerializeableLinkedResource> _linkedResources = new List<SerializeableLinkedResource>();
        private readonly TransferEncoding _transferEncoding;

        public SerializeableAlternateView(AlternateView alternativeView)
        {
            _baseUri = alternativeView.BaseUri;
            _contentId = alternativeView.ContentId;
            _contentType = new SerializeableContentType(alternativeView.ContentType);
            _transferEncoding = alternativeView.TransferEncoding;

            if (alternativeView.ContentStream != null)
            {
                byte[] bytes = new byte[alternativeView.ContentStream.Length];

                alternativeView.ContentStream.Read(bytes, 0, bytes.Length);
                _contentStream = new MemoryStream(bytes);
            }

            foreach (var lr in alternativeView.LinkedResources)
            {
                _linkedResources.Add(new SerializeableLinkedResource(lr));
            }
        }

        public AlternateView GetAlternateView()
        {
            var sav = new AlternateView(_contentStream)
            {
                BaseUri = _baseUri,
                ContentId = _contentId,
                ContentType = _contentType.GetContentType(),
                TransferEncoding = _transferEncoding,
            };

            foreach (var linkedResource in _linkedResources)
            {
                sav.LinkedResources.Add(linkedResource.GetLinkedResource());
            }

            return sav;
        }
    }
}
