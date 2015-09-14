using System;
using Xamarin.Forms.Maps;

namespace XamMapz
{
    public class MapViewChangedEventArgs : EventArgs
    {
        public MapSpan Span { get; private set; }
        public float ZoomLevel { get; private set; }

        public MapViewChangedEventArgs(MapSpan span, float zoomLevel)
        {
            Span = span;
            ZoomLevel = zoomLevel;
        }
    }
}
