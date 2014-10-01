using System;
using System.Net.Mime;

namespace CompositeC1Contrib.Email.Serialization
{
    [Serializable]
    public class SerializeableContentDisposition
    {
        private readonly DateTime _creationDate;
        private readonly string _dispositionType;
        private readonly string _fileName;
        private readonly bool _inline;
        private readonly DateTime _modificationDate;
        private readonly SerializeableCollection _parameters;
        private readonly DateTime _readDate;
        private readonly long _size;

        public SerializeableContentDisposition(ContentDisposition contentDisposition)
        {
            _creationDate = contentDisposition.CreationDate;
            _dispositionType = contentDisposition.DispositionType;
            _fileName = contentDisposition.FileName;
            _inline = contentDisposition.Inline;
            _modificationDate = contentDisposition.ModificationDate;
            _parameters = new SerializeableCollection(contentDisposition.Parameters);
            _readDate = contentDisposition.ReadDate;
            _size = contentDisposition.Size;
        }

        public void CopyTo(ContentDisposition contentDisposition)
        {
            contentDisposition.CreationDate = _creationDate;
            contentDisposition.DispositionType = _dispositionType;
            contentDisposition.FileName = _fileName;
            contentDisposition.Inline = _inline;
            contentDisposition.ModificationDate = _modificationDate;
            contentDisposition.ReadDate = _readDate;
            contentDisposition.Size = _size;

            _parameters.CopyTo(contentDisposition.Parameters);
        }
    }
}
