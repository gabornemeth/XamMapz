//
// MapElementDictionary.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Collections.Generic;
using System.Linq;

namespace XamMapz
{
    /// <summary>
    /// Manages association of a Xamarin Forms map element type to a native one
    /// </summary>
    public class MapElementDictionary<TAbstract, TNative>
    {
        private Dictionary<TNative, TAbstract> _nativeDict = new Dictionary<TNative, TAbstract>();
        private Dictionary<TAbstract, TNative> _dict = new  Dictionary<TAbstract, TNative>();

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

        /// <summary>
        /// Gets the association as an enumerable collection of Xamarin Forms Maps type.
        /// </summary>
        /// <returns>The enumerable collection</returns>
        public IEnumerable<TAbstract> AsEnumerable()
        {
            return new Enumerator<TAbstract>(this);
        }

        /// <summary>
        /// Removes all the associations
        /// </summary>
        public void Clear()
        {
            _dict.Clear();
            _nativeDict.Clear();
        }

        /// <summary>
        /// Adds a new association or updates a current one.
        /// </summary>
        /// <param name="pin">Xamarin Forms Maps element.</param>
        /// <param name="nativePin">Native element.</param>
        public void AddOrUpdate(TAbstract item, TNative nativeItem)
        {
            if (_dict.ContainsKey(item) == false)
                _dict.Add(item, nativeItem);
            else
            {
                if (_dict.ContainsValue(nativeItem)) // if nativeItem is present with another key, remove that
                {
                    _dict.Remove(_dict.First(element => element.Value.Equals(nativeItem)).Key);
                }
                _dict[item] = nativeItem; // add with the new key
            }

            if (_nativeDict.ContainsKey(nativeItem) == false)
                _nativeDict.Add(nativeItem, item);
            else
            {
                // if item is present with another key, remove that
                if (_nativeDict.ContainsValue(item))
                {
                    _nativeDict.Remove(_nativeDict.First(element => element.Value.Equals(item)).Key);
                }
                _nativeDict[nativeItem] = item;
            }
        }

        /// <summary>
        /// Remove the specified association.
        /// </summary>
        /// <param name="item">Association identified by <see cref="T"/> .</param>
        public void Remove(TAbstract item)
        {
            var nativeItem = GetNative(item);
            _dict.Remove(item);
            if (nativeItem != null)
                _nativeDict.Remove(nativeItem);
        }

        /// <summary>
        /// Remove the specified association
        /// </summary>
        /// <param name="nativeItem">Association identified by a Native item.</param>
        public void Remove(TNative nativeItem)
        {
            if (_nativeDict.ContainsKey(nativeItem) == false)
                return;

            var item = _nativeDict[nativeItem];
            _dict.Remove(item);
            _nativeDict.Remove(nativeItem);
        }

        /// <summary>
        /// Gets the Xamarin Forms Maps part of an association
        /// </summary>
        /// <param name="nativeItem">The native item.</param>
        public TAbstract Get(TNative nativeItem)
        {
            if (_nativeDict.ContainsKey(nativeItem))
                return _nativeDict[nativeItem];

            return default(TAbstract);
        }

        /// <summary>
        /// Gets the native part of an association.
        /// </summary>
        /// <returns>The native item.</returns>
        /// <param name="item">The Xamarin Forms Maps item.</param>
        public TNative GetNative(TAbstract item)
        {
            if (_dict.ContainsKey(item))
                return _dict[item];

            return default(TNative);
        }
          
    }
}

