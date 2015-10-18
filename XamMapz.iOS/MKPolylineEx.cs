using System;
using MapKit;

namespace XamMapz.iOS
{
    public class MKPolylineEx : MKPolyline
    {
        public MapPolyline _polyline { get; private set; }

        public MKPolylineEx(MapPolyline polyline)
        {
            _polyline = polyline;
        }
    }
}

