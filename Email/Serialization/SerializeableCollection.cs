using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace CompositeC1Contrib.Email.Serialization
{
    [Serializable]
    public class SerializeableCollection
    {
        private readonly IDictionary<string, string> _collection = new Dictionary<string, string>();

        public SerializeableCollection() { }

        public SerializeableCollection(NameValueCollection coll)
        {
            foreach (string key in coll.Keys)
            {
                _collection.Add(key, coll[key]);
            }
        }

        public SerializeableCollection(StringDictionary coll)
        {
            foreach (string key in coll.Keys)
            {
                _collection.Add(key, coll[key]);
            }
        }

        public void CopyTo(NameValueCollection scol)
        {
            foreach (string key in _collection.Keys)
            {
                scol.Add(key, _collection[key]);
            }
        }

        public void CopyTo(StringDictionary scol)
        {
            foreach (string key in _collection.Keys)
            {
                try
                {
                    if (scol.ContainsKey(key))
                    {
                        scol[key] = _collection[key];
                    }
                    else
                    {
                        scol.Add(key, _collection[key]);
                    }
                }
                catch (FormatException) { }
            }
        }
    }
}
