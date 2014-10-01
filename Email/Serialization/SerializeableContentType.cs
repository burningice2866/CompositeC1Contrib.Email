using System;
using System.Net.Mime;

namespace CompositeC1Contrib.Email.Serialization
{
    [Serializable]
    internal class SerializeableContentType
    {
        private readonly string _boundary;
        private readonly string _charSet;
        private readonly string _mediaType;
        private readonly string _name;
        private readonly SerializeableCollection _parameters;

        public SerializeableContentType(ContentType contentType)
        {
            _boundary = contentType.Boundary;
            _charSet = contentType.CharSet;
            _mediaType = contentType.MediaType;
            _name = contentType.Name;
            _parameters = new SerializeableCollection(contentType.Parameters);
        }

        public ContentType GetContentType()
        {
            var sct = new ContentType()
            {
                Boundary = _boundary,
                CharSet = _charSet,
                MediaType = _mediaType,
                Name = _name,
            };

            _parameters.CopyTo(sct.Parameters);

            return sct;
        }
    }
}
