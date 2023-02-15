using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Utility.Toolkit.Collections
{
    public class IndexedCollection
    {
        private ArrayList _index = new ArrayList();
        private ArrayList _stuff = new ArrayList();

        public void Add(string key, object o)
        {
            if (_index.IndexOf(key) != -1)
                throw new Exception("The key already exists in the array.");
            
            _index.Add(key);
            _stuff.Add(o);
        }

        public object GetItem(string key)
        {
            int itemIndex = _index.IndexOf(key);
            return _stuff[itemIndex];
        }

        public object[] GetItems()
        {
            return _stuff.ToArray();
        }

        public int Count()
        {
            return _stuff.Count;
        }
    }
}
