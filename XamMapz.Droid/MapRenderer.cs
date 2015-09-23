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

[assembly: ExportRenderer(typeof(XamMapz.Map), typeof(XamMapz.Droid.MapRenderer))]
namespace XamMapz.Droid
{
    /// <summary>
    /// Map renderer for Android - based on Xamarin Forms' MapRenderer
    /// Using Google Maps of course
    /// </summary>
    public class MapRenderer : Xamarin.Forms.Maps.Android.MapRenderer, IMapExRenderer
    {
        /// <summary>
        /// Displayed markers
        /// </summary>
        protected Dictionary<MapPin, Marker> Markers { get; private set; }

        private Dictionary<MapPolyline, PolylineAdv> _polylines = new Dictionary<MapPolyline, PolylineAdv>();

        public MapRenderer()
        {
            Markers = new Dictionary<MapPin, Marker>();
        }

        ~MapRenderer()
        {
            System.Diagnostics.Debug.WriteLine("MapExRenderer finalized.");
        }

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed == false && disposing)
            {
                _disposed = true;
                UnbindFromElement(Element as Map);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// View in Xamarin Forms
        /// </summary>
        private Map MapEx
        {
            get
            {
                return Element as Map;
            }
        }

        private bool _initialized;

        private void UnbindFromElement(Map map)
        {
            if (map != null)
            {
                MessagingCenter.Unsubscribe<Map, MapMessage>(this, MapMessage.Message);
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

        private void BindToElement(Map map)
        {
            if (map != null)
            {
                NativeMap.CameraChange += NativeMap_CameraChange;
                NativeMap.MarkerClick += NativeMap_MarkerClick;
                map.PinsInternal.CollectionChanged += OnPinsCollectionChanged;
                map.PolylinesInternal.CollectionChanged += OnPolylinesCollectionChanged;
                MessagingCenter.Subscribe<XamMapz.Map, MapMessage>(this, MapMessage.Message, (map1, message) =>
                {
                    OnMapMessage(map1, message);
                });
            }
        }

        protected virtual void OnMapMessage(Map map, MapMessage message)
        {
            if (message is ZoomMessage)
            {
                //var msg = (ZoomMessage)message;
                UpdateRegion();
            }
            else if (message is MapProjectMessage)
            {
                var msg = (MapProjectMessage)message;
                var screenPos = NativeMap.Projection.ToScreenLocation(msg.Position.ToLatLng());
                msg.ScreenPosition = new Point(screenPos.X, screenPos.Y);
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

        private void polyline_PositionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var polyline = sender as MapPolyline;
            if (polyline == null)
                return;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // modify the points
                var route = _polylines[polyline];
                if (route != null)
                {
                    foreach (Position pos in e.NewItems)
                    {
                        using (var latlng = pos.ToLatLng())
                        {
                            route.Add(latlng);
                        }
                    }
                }
            }
            else
            {
                // rebuild polyline - this is slow
                RemovePolyline(polyline);
                AddPolyline(polyline);
            }
        }

        void polyline_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var polyline = sender as MapPolyline;
            if (polyline == null)
                return;

            var line = _polylines[polyline];
            if (e.PropertyName == MapPolyline.ColorProperty.PropertyName)
            {
                // change color of the polyline
                line.Color = polyline.Color.ToAndroid().ToArgb();
            }
            else if (e.PropertyName == MapPolyline.ZIndexProperty.PropertyName)
            {
                // change Z-index of the polyline
                line.ZIndex = polyline.ZIndex;
            }
            else if (e.PropertyName == MapPolyline.PositionsProperty)
            {
                RemovePolyline(polyline);
                AddPolyline(polyline);
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
            OnPinPropertyChanged(pin, marker, e);
        }

        protected virtual void OnPinPropertyChanged(MapPin pin, Marker marker, PropertyChangedEventArgs e)
        {
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
            OnCameraChange(bound, pos);

            MessagingCenter.Send<IMapExRenderer, MapMessage>(this, MapMessage.RendererMessage, new ViewChangeMessage { Span = bound, ZoomLevel = pos.Zoom });
        }

        protected virtual void OnCameraChange(MapSpan span, CameraPosition pos)
        {

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
                UnbindFromElement(e.OldElement as Map);
            }
            if (e.NewElement != null)
            {
                BindToElement(e.NewElement as Map);
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

        private void RemovePolyline(MapPolyline polyline)
        {
            if (_polylines.ContainsKey(polyline) == false)
                return;

            RemovePolylineFromMap(polyline);
            _polylines.Remove(polyline);
        }

        private void RemovePolylineFromMap(MapPolyline polyline)
        {
            if (_polylines.ContainsKey(polyline))
            {
                var line = _polylines[polyline];
                // remove the old polyline from the map
                line.RemoveFromMap();
                line.Dispose();
            }
        }

        /// <summary>
        /// Remove all polylines
        /// </summary>
        private void ClearPolylines()
        {
            foreach (var polylineEntry in _polylines)
            {
                UnbindPolyline(polylineEntry.Key);
                RemovePolylineFromMap(polylineEntry.Key);
            }
            _polylines.Clear();
        }

        private PolylineOptions CreatePolylineOptions(MapPolyline polyline)
        {
            var op = new PolylineOptions();
            op.InvokeColor(polyline.Color.ToAndroid().ToArgb());
            op.InvokeWidth((float)polyline.Width);
            op.InvokeZIndex(polyline.ZIndex);
            return op;
        }

        private void AddPolyline(MapPolyline polyline)
        {
            // add the new one
            using (var op = CreatePolylineOptions(polyline))
            {
                foreach (var pt in polyline.Positions)
                {
                    op.Add(pt);
                }
                // add the last polyline segment
                var line = PolylineAdv.Add(NativeMap, op);
                _polylines.Add(polyline, line);
            }
        }

        private Polyline AddPolyline(PolylineOptions option)
        {
            var polyline = NativeMap.AddPolyline(option);
            polyline.Color = option.Color;
            polyline.Width = option.Width;
            return polyline;
        }

        private void UpdateGoogleMap(Action<Map> action)
        {
            if (Control == null)
                return; // this should not occur

            if (!_initialized && !Control.IsLaidOut)
                return;

            var formsMap = (Map)Element;

            action(formsMap);
        }

        private void AddMarker(MapPin pin)
        {
            using (var op = new MarkerOptions())
            {
                op.SetTitle(pin.Label);
                op.SetPosition(pin.Position.ToLatLng());
                op.SetIcon(BitmapDescriptorFactory.DefaultMarker(pin.Color.ToAndroidMarkerHue()));
                var marker = NativeMap.AddMarker(op);
                pin.Id = marker.Id;
                Markers.Add(pin, marker);
            }
        }

        private void RemoveMarker(MapPin pin)
        {
            if (Markers.ContainsKey(pin) == false)
                return;

            var markerToRemove = Markers[pin];

            markerToRemove.Remove();
            markerToRemove.Dispose();
            UnbindPin(pin);
            Markers.Remove(pin);
        }

        private void ClearMarkers()
        {
            // Remove all markers
            foreach (var marker in Markers)
            {
                marker.Value.Remove();
                marker.Value.Dispose();
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
