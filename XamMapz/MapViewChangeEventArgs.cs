//
// MapViewChangeEventArgs.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using Microsoft.Maui.Maps;

namespace XamMapz
{
    /// <summary>
    /// Event arguments about change in view
    /// </summary>
    public class MapViewChangeEventArgs : EventArgs
    {
        public MapSpan Span { get; private set; }
        public float ZoomLevel { get; private set; }

        public MapViewChangeEventArgs(MapSpan span, float zoomLevel)
        {
            Span = span;
            ZoomLevel = zoomLevel;
        }
    }
}
