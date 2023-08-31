//
// PolylineAdv.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Collections.Generic;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;

namespace XamMapz.Droid
{
    /// <summary>
    /// Advanced polyline optimized for frequent modifications.
    /// Stored as collection of shorter polylines to achieve this.
    /// </summary>
    class PolylineAdv : IDisposable
    {
        private const int PolylineSegmentLength = 50; // make polyline segments max. 100 points long

        private List<Polyline> _polylines = new List<Polyline>();
        private PolylineOptions _options;
        private GoogleMap _map;

        private PolylineAdv(GoogleMap map, PolylineOptions options)
        {
            _map = map;
            _options = CloneOptions(options);
        }

        public int Color
        {
            set
            {
                foreach (var polyline in _polylines)
                    polyline.Color = value;
                _options.InvokeColor(value);
            }
        }

        public float ZIndex
        {
            set
            {
                foreach (var polyline in _polylines)
                    polyline.ZIndex = value;
            }
        }

        private static PolylineOptions CloneOptions(PolylineOptions op)
        {
            var options = new PolylineOptions();
            options.InvokeColor(op.Color);
            options.InvokeWidth(op.Width);
            options.InvokeZIndex(op.ZIndex);
            return options;
        }

        public static PolylineAdv Add(GoogleMap map, PolylineOptions op)
        {
            var polyline = new PolylineAdv(map, op);
            var idxStart = 0;
            while (idxStart < op.Points.Count)
            {
                using (var options = CloneOptions(op))
                {
                    for (var i = idxStart; i < Math.Min(idxStart + PolylineSegmentLength + 1, op.Points.Count); i++)
                    {
                        options.Add(op.Points[i]);
                    }
                    polyline._polylines.Add(map.AddPolyline(options));
                }

                idxStart += PolylineSegmentLength;
            }

            return polyline;
        }

        public void Add(LatLng latLng)
        {
            if (_options == null)
                throw new Exception(string.Format("{0} must be initialized.", GetType().Name));

            if (_polylines.Count == 0)
            {
                // no position yet
                var op = CloneOptions(_options);
                op.Points.Add(latLng);
                var polyline = _map.AddPolyline(op);
                _polylines.Add(polyline);
            }
            else
            {
                // add position to the las polyline
                var polyline = _polylines.Last();
                var points = polyline.Points;
                points.Add(latLng);
                polyline.Points = points;
                if (points.Count == PolylineSegmentLength + 1)
                {
                    // Polyline is full
                    var op = CloneOptions(_options);
                    op.Points.Add(latLng);
                    var nextPolyline = _map.AddPolyline(op);
                    _polylines.Add(nextPolyline);
                }
            }
        }

        public void RemoveFromMap()
        {
            foreach (var polyline in _polylines)
            {
                polyline.Remove();
            }
        }

        public void Dispose()
        {
            foreach (var polyline in _polylines)
            {
                polyline.Dispose();
            }
        }
    }
}
