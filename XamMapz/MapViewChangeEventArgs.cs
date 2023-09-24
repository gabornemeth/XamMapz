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
        public MapSpan Span { get; }

        public MapViewChangeEventArgs(MapSpan span)
        {
            Span = span;
        }
    }
}
