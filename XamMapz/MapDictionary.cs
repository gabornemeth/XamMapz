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
    internal class MapDictionary<TPin, TPolyline>
    {
        public MapElementDictionary<PinX, TPin> Pins { get; private set; }
        public MapElementDictionary<PolylineX, TPolyline> Polylines { get; private set; }

        public MapDictionary()
        {
            Pins = new MapElementDictionary<PinX, TPin>();
            Polylines = new MapElementDictionary<PolylineX, TPolyline>();
        }

        public void Clear()
        {
            Pins.Clear();
            Polylines.Clear();
        }
    }
}

