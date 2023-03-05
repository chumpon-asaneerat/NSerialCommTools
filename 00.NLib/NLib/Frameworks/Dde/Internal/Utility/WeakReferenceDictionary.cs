namespace NLib.Dde
{
    using System;
    using System.Collections.Generic;

    internal sealed class WeakReferenceDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TValue : class
    {
        private IDictionary<TKey, WeakReference> _Storage = new Dictionary<TKey, WeakReference>();

        public WeakReferenceDictionary()
        {
        }

        public void Add(TKey key, TValue value)
        {
            Purge();
            _Storage.Add(key, new WeakReference(value));
        }

        public bool ContainsKey(TKey key)
        {
            return _Storage.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _Storage.Keys; }
        }

        public bool Remove(TKey key)
        {
            return _Storage.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = null;
            if (_Storage.ContainsKey(key))
            {
                value = _Storage[key].Target as TValue;
                if (value != null)
                {
                    return true;
                }
            }
            return false;
        }

        public ICollection<TValue> Values
        {
            get { return new MyValueCollection(this); }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (_Storage.ContainsKey(key))
                {
                    TValue value = _Storage[key].Target as TValue;
                    if (value != null)
                    {
                        return value;
                    }
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    Purge();
                    _Storage[key] = new WeakReference(value);
                }
                else
                {
                    _Storage.Remove(key);
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Purge();
            _Storage.Add(item.Key, new WeakReference(item.Value));
        }

        public void Clear()
        {
            _Storage.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (_Storage.ContainsKey(item.Key))
            {
                TValue value = _Storage[item.Key].Target as TValue;
                if (value != null)
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            int index = 0;
            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                array[arrayIndex + index] = kvp;
                index++;
            }
        }

        public int Count
        {
            get { return _Storage.Count; }
        }

        public bool IsReadOnly
        {
            get { return _Storage.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (KeyValuePair<TKey, WeakReference> kvp in _Storage)
            {
                TValue value = kvp.Value.Target as TValue;
                if (value != null)
                {
                    yield return new KeyValuePair<TKey, TValue>(kvp.Key, value);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Purge()
        {
            List<TKey> dead = new List<TKey>();
            foreach (KeyValuePair<TKey, WeakReference> kvp in _Storage)
            {
                if (!kvp.Value.IsAlive)
                {
                    dead.Add(kvp.Key);
                }
            }
            foreach (TKey key in dead)
            {
                _Storage.Remove(key);
            }
        }

        private sealed class MyValueCollection : ICollection<TValue>
        {
            private WeakReferenceDictionary<TKey, TValue> _Parent = null;

            public MyValueCollection(WeakReferenceDictionary<TKey, TValue> parent)
            {
                _Parent = parent;
            }

            public void Add(TValue item)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public void Clear()
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public bool Contains(TValue item)
            {
                foreach (TValue value in this)
                {
                    if (value == item)
                    {
                        return true;
                    }
                }
                return false;
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                int index = 0;
                foreach (TValue value in this)
                {
                    array[arrayIndex + index] = value;
                    index++;
                }
            }

            public int Count
            {
                get { return _Parent._Storage.Values.Count; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool Remove(TValue item)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                foreach (WeakReference wr in _Parent._Storage.Values)
                {
                    TValue value = wr.Target as TValue;
                    if (value != null)
                    {
                        yield return value;
                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

        } // class

    } // class

} // namespace