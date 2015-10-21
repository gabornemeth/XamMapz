//
// MapDictionary.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Collections.Generic;

namespace XamMapz
{
    /// <summary>
    /// Dictionary for native and Xamarin Forms map elements
    /// This is suitable for quick retreival of together mapped elements
    /// </summary>
    public class MapDictionary<TPin, TPolyline>
    {
        public MapElementDictionary<MapPin, TPin> Pins { get; private set; }
        public MapElementDictionary<MapPolyline, TPolyline> Polylines { get; private set; }

        public MapDictionary()
        {
            Pins = new MapElementDictionary<MapPin, TPin>();
            Polylines = new MapElementDictionary<MapPolyline, TPolyline>();
        }

        public void Clear()
        {
            Pins.Clear();
            Polylines.Clear();
        }
    }
}

