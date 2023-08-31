//
// MkPolylineEx.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using MapKit;
using CoreLocation;

namespace XamMapz.iOS
{
    /// <summary>
    /// Advanced polyline - optimized for frequent updates.
    /// Using a collection of shorter polylines to achieve this.
    /// </summary>
    public class MKPolylineEx : IDisposable
    {
        private const int PolylineSegmentLength = 50;
        // make polyline segments max. 100 points long

        private List<MKPolyline> _polylines = new List<MKPolyline>();
        private MKMapView _map;
        private List<MKPolylineEx> _collection;

        public MKPolylineEx(MKMapView map, List<MKPolylineEx> polylines, CLLocationCoordinate2D[] coordinates)
        {
            _map = map;
            _collection = polylines;
            var idxStart = 0;
            while (idxStart < coordinates.Length)
            {
                var coordsToAdd = new List<CLLocationCoordinate2D>();
                for (var i = idxStart; i < Math.Min(idxStart + PolylineSegmentLength + 1, coordinates.Length); i++)
                {
                    coordsToAdd.Add(coordinates[i]);
                }
                var polylineToAdd = MKPolyline.FromCoordinates(coordsToAdd.ToArray());
                _polylines.Add(polylineToAdd);

                idxStart += PolylineSegmentLength;
            }
        }

        private UIColor _color;

        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>The color.</value>
        public UIColor Color
        {
            get
            {
                return _color;
            }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    foreach (var polyline in _polylines)
                    {
                        var view = _map.ViewForOverlay(polyline) as MKPolylineView;
                        if (view != null)
                            view.StrokeColor = value;
                    }
                }
            }
        }

        private float _zIndex = 1;

        /// <summary>
        /// Z index of the polyline
        /// </summary>
        /// <value>The index of the Z.</value>
        public float ZIndex
        {
            get
            {
                return _zIndex;
            }
            set
            {
                if (_zIndex != value)
                {
                    _zIndex = value;
                    foreach (var polyline in _polylines)
                    {
                        _map.RemoveOverlay(polyline);
                    }
                    var newIndex = GetOverlayIndex(value);
                    foreach (var polyline in _polylines)
                    {
                        _map.InsertOverlay(polyline, newIndex++);
                    }
                }
            }
        }

        private int GetOverlayIndex(float zIndex)
        {
            int i = 0;
            if (_map.Overlays != null)
            {
                for (i = 0; i < _map.Overlays.Length; i++)
                {
                    var overlay = _map.Overlays[i];
                    if (overlay is MKPolyline)
                    {
                        foreach (var polyline in _collection)
                        {
                            if (_collection.Contains(polyline) == false)
                                continue;

                            if (polyline.ZIndex > zIndex)
                                return i;
                        }
                    }
                }
            }

            return i;
        }

        internal MKPolylineView OnCreateView(MKPolyline polyline)
        {
            foreach (var p in _polylines)
            {
                if (p == polyline)
                {
                    var polylineView = new MKPolylineView(polyline);
                    polylineView.StrokeColor = _color;
                    return polylineView;
                }
            }

            return null;
        }

        public void InsertCoordinate(int index, CLLocationCoordinate2D coord)
        {
            var indexOverlay = GetOverlayIndex(ZIndex);
            if (_polylines.Count == 0)
            {
                // no position yet
                var polyline = MKPolyline.FromCoordinates(new[] { coord });
                _map.InsertOverlay(polyline, indexOverlay);
                _polylines.Add(polyline);
            }
            else
            {
                // find the polyline to extend with the new coordinate
                // add position to the las polyline
                var polyline = _polylines.Last();
                _map.RemoveOverlay(polyline);
                _polylines.Remove(polyline);
                var points = new List<CLLocationCoordinate2D>(polyline.GetCoordinates(0, (int)polyline.PointCount));
                points.Add(coord);
                polyline = MKPolyline.FromCoordinates(points.ToArray());
                _polylines.Add(polyline);
                _map.InsertOverlay(polyline, indexOverlay);

                if (polyline.PointCount == PolylineSegmentLength + 1)
                {
                    // Polyline is full
                    var nextPolyline = MKPolyline.FromCoordinates(new[] { coord });
                    _map.InsertOverlay(nextPolyline, indexOverlay);
                    _polylines.Add(nextPolyline);
                }
            }
        }

        public void AddToMap()
        {
            var index = GetOverlayIndex(ZIndex);
            foreach (var polyline in _polylines)
            {
                _map.InsertOverlay(polyline, index++);
            }
        }

        public void RemoveFromMap()
        {
            foreach (var polyline in _polylines)
            {
                _map.RemoveOverlay(polyline);
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
