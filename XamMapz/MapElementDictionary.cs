using System;
using System.Collections.Generic;

namespace XamMapz
{
    /// <summary>
    /// Maps a type of Xamarin Forms map element to a native one
    /// </summary>
    public class MapElementDictionary<T, TNative>
    {
        private Dictionary<TNative, T> _nativeDict = new Dictionary<TNative, T>();
        private Dictionary<T, TNative> _dict = new  Dictionary<T, TNative>();

        private class Enumerator<T> : IEnumerable<T>
        {
            private MapElementDictionary<T, TNative> _dict;

            public Enumerator(MapElementDictionary<T, TNative> dict)
            {
                _dict = dict;
            }

            private IEnumerator<T> GetEnumeratorInternal()
            {
                var en = _dict._dict.GetEnumerator();
                while (en.MoveNext())
                {
                    yield return en.Current.Key;
                }
            }

            #region IEnumerable implementation

            public IEnumerator<T> GetEnumerator()
            {
                return GetEnumeratorInternal();
            }

            #endregion

            #region IEnumerable implementation

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumeratorInternal();
            }

            #endregion
        }

        public IEnumerable<T> AsEnumerable()
        {
            return new Enumerator<T>(this);
        }

        public void Clear()
        {
            _dict.Clear();
            _nativeDict.Clear();
        }

        /// <summary>
        /// Adds a new pin.
        /// </summary>
        /// <param name="pin">Xamarin Forms Maps Pin.</param>
        /// <param name="nativePin">Native pin.</param>
        public void Add(T item, TNative nativeItem)
        {
            _dict.Add(item, nativeItem);
            _nativeDict.Add(nativeItem, item);
        }

        public void Remove(T item)
        {
            var nativeItem = GetNative(item);
            _dict.Remove(item);
            if (nativeItem != null)
                _nativeDict.Remove(nativeItem);
        }

        public void Remove(TNative nativeItem)
        {
            if (_nativeDict.ContainsKey(nativeItem) == false)
                return;

            var pin = _nativeDict[nativeItem];
            _dict.Remove(pin);
            _nativeDict.Remove(nativeItem);
        }

        public T Get(TNative nativeItem)
        {
            if (_nativeDict.ContainsKey(nativeItem))
                return _nativeDict[nativeItem];

            return default(T);
        }

        public TNative GetNative(T item)
        {
            if (_dict.ContainsKey(item))
                return _dict[item];

            return default(TNative);
        }
          
    }
}

