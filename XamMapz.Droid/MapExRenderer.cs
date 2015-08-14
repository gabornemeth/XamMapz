//
// MapExRenderer.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Maps;
using XamMapz;
using XamMapz.Droid;
using XamMapz.Messaging;
using XamMapz.Extensions;

[assembly: ExportRenderer(typeof(MapEx), typeof(MapExRenderer))]
namespace XamMapz.Droid
{
    /// <summary>
    /// Map renderer for Android - based on Xamarin Forms' MapRenderer
    /// Using Google Maps of course
    /// </summary>
    class MapExRenderer : MapRenderer, IMapExRenderer
    {
        /// <summary>
        /// Displayed markers
        /// </summary>
        protected Dictionary<MapPin, Marker> Markers { get; private set; }

        private Dictionary<MapPolyline, List<Polyline>> _polylines = new Dictionary<MapPolyline, List<Polyline>>();

        public MapExRenderer()
        {
            Markers = new Dictionary<MapPin, Marker>();
        }

        ~MapExRenderer()
        {
            System.Diagnostics.Debug.WriteLine("MapExRenderer finalized.");
        }

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed == false && disposing)
            {
                _disposed = true;
                UnbindFromElement(Element as MapEx);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// View in Xamarin Forms
        /// </summary>
        private MapEx MapEx
        {
            get
            {
                return Element as MapEx;
            }
        }

        private bool _initialized;

        private void UnbindFromElement(MapEx map)
        {
            if (map != null)
            {
                MessagingCenter.Unsubscribe<MapEx, MapMessage>(this, MapMessage.Message);
                NativeMap.CameraChange -= NativeMap_CameraChange;
                NativeMap.MarkerClick -= NativeMap_MarkerClick;
                map.PinsInternal.CollectionChanged -= OnPinsCollectionChanged;
                map.PolylinesInternal.CollectionChanged -= OnPolylinesCollectionChanged;
                foreach (var polyline in map.Polylines)
                {
                    polyline.PositionChanged -= polyline_PositionChanged;
                    polyline.PropertyChanged -= polyline_PropertyChanged;
                }
            }
        }

        private void BindToElement(MapEx map)
        {
            if (map != null)
            {
                NativeMap.CameraChange += NativeMap_CameraChange;
                NativeMap.MarkerClick += NativeMap_MarkerClick;
                map.PinsInternal.CollectionChanged += OnPinsCollectionChanged;
                map.PolylinesInternal.CollectionChanged += OnPolylinesCollectionChanged;
                MessagingCenter.Subscribe<MapEx, MapMessage>(this, MapMessage.Message, OnMapMessage);
            }
        }

        private void OnMapMessage(MapEx map, MapMessage message)
        {
            if (message is ZoomMessage)
            {
                //var msg = (ZoomMessage)message;
                UpdateRegion();
            }
        }

        private void BindPolyline(MapPolyline polyline)
        {
            polyline.PropertyChanged += polyline_PropertyChanged;
            polyline.PositionChanged += polyline_PositionChanged;
        }

        private void UnbindPolyline(MapPolyline polyline)
        {
            polyline.PropertyChanged -= polyline_PropertyChanged;
            polyline.PositionChanged -= polyline_PositionChanged;
        }

        private void OnPolylinesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // new polyline(s)
                foreach (MapPolyline polyline in e.NewItems)
                {
                    AddPolyline(polyline);
                    BindPolyline(polyline);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // deleted polyline(s)
                foreach (MapPolyline polyline in e.OldItems)
                {
                    RemovePolyline(polyline);
                    UnbindPolyline(polyline);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearPolylines();
            }
        }

        private void polyline_PositionChanged(object sender, MapRoutePositionChangeEventArgs e)
        {
            var polyline = sender as MapPolyline;
            if (polyline == null)
                return;

            RemovePolyline(polyline);
            AddPolyline(polyline);
        }

        void polyline_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var polyline = sender as MapPolyline;
            if (polyline == null)
                return;

            if (e.PropertyName == MapPolyline.ColorProperty.PropertyName)
            {
                // change color of the polyline
                foreach (var line in _polylines[polyline])
                {
                    line.Color = polyline.Color.ToAndroid().ToArgb();
                    line.ZIndex = 255;
                }
            }
        }

        private void BindPin(MapPin pin)
        {
            pin.PropertyChanged += pin_PropertyChanged;
        }

        private void UnbindPin(MapPin pin)
        {
            pin.PropertyChanged -= pin_PropertyChanged;
        }

        private void OnPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // new pin(s)
                foreach (MapPin pin in e.NewItems)
                {
                    AddMarker(pin);
                    BindPin(pin);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // deleted pin(s)
                foreach (MapPin pin in e.OldItems)
                {
                    RemoveMarker(pin);
                    UnbindPin(pin);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearMarkers();
            }
        }

        void pin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Property of a pin has been changed.
            var pin = sender as MapPin;
            if (pin == null)
                return;

            // retrieve Google Maps marker
            if (Markers.ContainsKey(pin) == false)
                return;

            var marker = Markers[pin];

            if (e.PropertyName == MapPin.ColorProperty.PropertyName)
            {
                marker.SetIcon(BitmapDescriptorFactory.DefaultMarker(pin.Color.ToAndroidMarkerHue()));
            }
            else if (e.PropertyName == MapPin.PositionProperty.PropertyName)
            {
                marker.Position = pin.Position.ToLatLng();
            }
        }

        void NativeMap_CameraChange(object sender, GoogleMap.CameraChangeEventArgs e)
        {
            UpdatePosition(e.Position);
        }

        private void UpdatePosition(CameraPosition pos)
        {
            // if any marker is shown, don't invoke camera change
            var northWest = NativeMap.Projection.FromScreenLocation(new Android.Graphics.Point(0, 0)).ToPosition();
            var center = pos.Target.ToPosition();
            var distanceInDegrees = northWest.DistanceFrom(center);

            var bound = new MapSpan(new Position(center.Latitude, center.Longitude),
                distanceInDegrees.Latitude * 2, distanceInDegrees.Longitude * 2);

            MessagingCenter.Send<IMapExRenderer, MapMessage>(this, MapMessage.RendererMessage, new ViewChangeMessage { Span = bound, ZoomLevel = pos.Zoom });
        }

        void NativeMap_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            var pin = MapEx.Pins.FirstOrDefault(p => p.Id == e.Marker.Id); // clicked pin
            if (pin == null)
                return;
            pin.OnClicked();
            e.Marker.ShowInfoWindow();
            e.Handled = true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // this never gets called
                UnbindFromElement(e.OldElement as MapEx);
            }
            if (e.NewElement != null)
            {
                BindToElement(e.NewElement as MapEx);
                NativeMap.TrafficEnabled = false;
            }
        }

        private void UpdatePolylines()
        {
            UpdateGoogleMap(formsMap =>
            {
                ClearPolylines();

                foreach (var polyline in formsMap.Polylines)
                {
                    AddPolyline(polyline);
                    BindPolyline(polyline);
                }
            });
        }

        private void RemovePolyline(MapPolyline route)
        {
            bool contains = _polylines.ContainsKey(route);
            var routeEntry = contains ? _polylines[route] : new List<Polyline>();
            // remove the old polyline from the map
            foreach (var polyline in routeEntry)
            {
                polyline.Remove();
            }
            routeEntry.Clear();
        }

        private void ClearPolylines()
        {
            // Remove all polylines
            foreach (var polylineEntry in _polylines)
            {
                UnbindPolyline(polylineEntry.Key);
                RemovePolyline(polylineEntry.Key);
            }
            _polylines.Clear();
        }

        private PolylineOptions CreatePolylineOptions(MapPolyline polyline)
        {
            var op = new PolylineOptions();
            op.InvokeColor(polyline.Color.ToAndroid().ToArgb());
            op.InvokeWidth((float)polyline.Width);
            return op;
        }

        private void AddPolyline(MapPolyline polyline)
        {
            bool contains = _polylines.ContainsKey(polyline);
            var routeEntry = contains ? _polylines[polyline] : new List<Polyline>();
            // add the new one
            const int polylineSegmentLength = 100; // make polyline segments max. 100 points long
            var num = 0;
            var op = CreatePolylineOptions(polyline);
            foreach (var pt in polyline.Positions)
            {
                op.Add(pt.ToLatLng());
                if (num++ == polylineSegmentLength)
                {
                    routeEntry.Add(AddPolyline(op));
                    op = CreatePolylineOptions(polyline);
                    op.Add(pt.ToLatLng());
                    num = 1;
                }
            }
            // add the last polyline segment
            if (op.Points.Count > 0)
                routeEntry.Add(AddPolyline(op));
            // add to the dictionary, if not contained yet
            if (!contains)
                _polylines.Add(polyline, routeEntry);
        }

        private Polyline AddPolyline(PolylineOptions option)
        {
            var polyline = NativeMap.AddPolyline(option);
            polyline.Color = option.Color;
            polyline.Width = option.Width;
            return polyline;
        }

        private void UpdateGoogleMap(Action<MapEx> action)
        {
            if (Control == null)
                return; // this should not occur

            if (!_initialized && !Control.IsLaidOut)
                return;

            var formsMap = (MapEx)Element;

            action(formsMap);
        }

        private void AddMarker(MapPin pin)
        {
            var op = new MarkerOptions();
            op.SetTitle(pin.Label);
            op.SetPosition(pin.Position.ToLatLng());
            op.InvokeIcon(BitmapDescriptorFactory.DefaultMarker(pin.Color.ToAndroidMarkerHue()));
            var marker = NativeMap.AddMarker(op);
            pin.Id = marker.Id;
            Markers.Add(pin, marker);
        }

        private void RemoveMarker(MapPin pin)
        {
            if (Markers.ContainsKey(pin) == false)
                return;

            var markerToRemove = Markers[pin];

            markerToRemove.Remove();
            Markers.Remove(pin);
        }

        private void ClearMarkers()
        {
            // Remove all markers
            foreach (var marker in Markers)
            {
                marker.Value.Remove();
                UnbindPin(marker.Key);
            }
            Markers.Clear();
        }

        private void UpdateMarkers()
        {
            ClearMarkers();
            foreach (var pin in MapEx.Pins)
            {
                AddMarker(pin);
            }
        }

        private void UpdateRegion()
        {
            UpdateGoogleMap(formsMap =>
            {
                var cameraUpdate = CameraUpdateFactory.NewLatLngBounds(MapEx.Region.ToLatLngBounds(), 0);
                NativeMap.MoveCamera(cameraUpdate);
            });
        }

        private void Update()
        {
            UpdateRegion();
            UpdatePolylines();
            UpdateMarkers();
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            if (!_initialized && r > 0 && b > 0)
            {
                Update();
                _initialized = true;
            }
        }
    }
}
