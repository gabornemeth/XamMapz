using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Maps;
using XamMapz;
using XamMapz.Droid.Renderers;
using XamMapz.Messaging;
using Color = Xamarin.Forms.Color;
using XamMapz.Droid.Extensions;
using XamMapz.Extensions;

[assembly: ExportRenderer(typeof(MapEx), typeof(MapExRenderer))]
namespace XamMapz.Droid.Renderers
{
    /// <summary>
    /// Extended map renderer for Android
    /// Using Google Maps of course
    /// </summary>
    class MapExRenderer : MapRenderer, IMapExRenderer
    {
        /// <summary>
        /// Displayed markers
        /// </summary>
        protected List<Marker> Markers { get; private set; }
        private CameraPosition _lastCameraPosition;
        private CameraUpdate _cameraUpdateInit;

        public MapExRenderer()
        {
            Markers = new List<Marker>();
            MessagingCenter.Subscribe<MapEx, MapMessage>(this, MapMessage.Message, (map, message) =>
            {
                if (message is ZoomMessage)
                {
                    var msg = (ZoomMessage)message;
                    var cameraUpdate = CameraUpdateFactory.NewLatLngBounds(msg.Bounds.ToLatLngBounds(), 0);
                    if (_initialized == false)
                        _cameraUpdateInit = cameraUpdate;
                    else
                    {
                        UpdateGoogleMap(formsMap =>
                        {
                            NativeMap.MoveCamera(cameraUpdate);
                        });
                    }
                }
            });
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
        private MapEx MapView
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
                map.PropertyChanged -= Element_PropertyChanged;
                //map.RouteChanged -= map_RouteChanged;
                NativeMap.CameraChange -= NativeMap_CameraChange;
                NativeMap.MarkerClick -= NativeMap_MarkerClick;
                map.PinsInternal.CollectionChanged -= OnPinsCollectionChanged;
                map.PolylinesInternal.CollectionChanged -= OnRoutesCollectionChanged;
            }
        }

        private void BindToElement(MapEx map)
        {
            if (map != null)
            {
                map.PropertyChanged += Element_PropertyChanged;
                //map.RouteChanged += map_RouteChanged;
                NativeMap.CameraChange += NativeMap_CameraChange;
                NativeMap.MarkerClick += NativeMap_MarkerClick;
                map.PinsInternal.CollectionChanged += OnPinsCollectionChanged;
                map.PolylinesInternal.CollectionChanged += OnRoutesCollectionChanged;
            }
        }

        private void OnRoutesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // new route(s)
                foreach (MapPolyline route in e.NewItems)
                {
                    AddRoute(route);
                    route.PropertyChanged += route_PropertyChanged;
                    route.PositionChanged += route_PositionChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // deleted route(s)
                foreach (MapPolyline route in e.OldItems)
                {
                    RemoveRoute(route);
                    route.PropertyChanged -= route_PropertyChanged;
                    route.PositionChanged -= route_PositionChanged;
                }
            }
        }

        private void route_PositionChanged(object sender, MapRoutePositionChangeEventArgs e)
        {
            var polyline = sender as MapPolyline;
            if (polyline == null)
                return;

            RemoveRoute(polyline);
            AddRoute(polyline);
        }

        void route_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var polyline = sender as MapPolyline;
            if (polyline == null)
                return;

            if (e.PropertyName == MapPolyline.ColorProperty.PropertyName)
            {
                // change color of the polyline
                foreach (var line in _routes[polyline])
                {
                    line.Color = polyline.Color.ToAndroid().ToArgb();
                    line.ZIndex = 255;
                }
            }
        }

        private void OnPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // new pin(s)
                foreach (MapPin pin in e.NewItems)
                {
                    AddMarker(pin);
                    pin.PropertyChanged += pin_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // deleted pin(s)
                foreach (MapPin pin in e.OldItems)
                {
                    RemoveMarker(pin);
                    pin.PropertyChanged -= pin_PropertyChanged;
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
            var marker = Markers.FirstOrDefault(m => m.Id == pin.Id);
            if (marker == null)
                return;

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
            _lastCameraPosition = e.Position;
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

            MessagingCenter.Send<IMapExRenderer, MapMessage>(this, MapMessage.RendererMessage,
                new ViewChangeMessage { Span = bound, ZoomLevel = pos.Zoom });
        }

        void NativeMap_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            var pin = MapView.Pins.FirstOrDefault(p => p.Id == e.Marker.Id); // clicked pin
            if (pin == null)
                return;
            pin.OnClicked();
            e.Marker.ShowInfoWindow();
            e.Handled = true;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            if (e.OldElement != null)
            {
                // this never gets called
                UnbindFromElement(e.OldElement as MapEx);
                base.OnElementChanged(e);
            }
            if (e.NewElement != null)
            {
                base.OnElementChanged(e);
                BindToElement(e.NewElement as MapEx);
                NativeMap.TrafficEnabled = false;
            }
        }

        void Element_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MapEx.CenterProperty.PropertyName)
                UpdateCenter();
            else if (e.PropertyName == MapEx.ZoomLevelProperty.PropertyName)
                UpdateZoomLevel();
        }

        //void Map_AddedToRoute(object sender, XDotNet.Geolocation.GeoPosition e)
        //{
        //    if (NativeMap == null)
        //        return;

        //    // add new point to the last polyline
        //    if (_polyline == null)
        //    {
        //        _polyline = NativeMap.AddPolyline(new PolylineOptions());
        //        _polyline.Color = Color.Red.ToAndroid().ToArgb();
        //        _polyline.Width = 10;
        //    }

        //    var points = _polyline.Points;
        //    points.Add(e.ToLatLng());
        //    _polyline.Points = points;
        //}

        private Dictionary<MapPolyline, List<Polyline>> _routes = new Dictionary<MapPolyline, List<Polyline>>();

        private void UpdateRoutes()
        {
            if (NativeMap == null || Control == null)
                return; // this should not occur

            NativeMap.Clear();
            GC.Collect();
            if (MapView.Polylines == null)
                return;

            //var androidMapView = (MapView)Control;
            if (Control.IsLaidOut == false)
                return;

            foreach (var route in MapView.Polylines)
            {
                AddRoute(route);
            }
        }

        private void RemoveRoute(MapPolyline route)
        {
            bool contains = _routes.ContainsKey(route);
            var routeEntry = contains ? _routes[route] : new List<Polyline>();
            // remove the old polyline from the map
            foreach (var polyline in routeEntry)
            {
                polyline.Remove();
            }
            routeEntry.Clear();
        }

        private void AddRoute(MapPolyline route)
        {
            bool contains = _routes.ContainsKey(route);
            var routeEntry = contains ? _routes[route] : new List<Polyline>();
            // add the new one
            const int polylineSegmentLength = 100; // make polyline segments max. 100 points long
            var num = 0;
            var op = new PolylineOptions();
            op.InvokeColor(route.Color.ToAndroid().ToArgb());
            foreach (var pt in route.Positions)
            {
                op.Add(pt.ToLatLng());
                if (num++ == polylineSegmentLength)
                {
                    routeEntry.Add(AddPolyline(op));
                    op = new PolylineOptions();
                    op.InvokeColor(route.Color.ToAndroid().ToArgb());
                    op.Add(pt.ToLatLng());
                    num = 1;
                }
            }
            // add the last polyline segment
            if (op.Points.Count > 0)
                routeEntry.Add(AddPolyline(op));
            // add to the dictionary, if not contained yet
            if (!contains)
                _routes.Add(route, routeEntry);
        }

        private Polyline AddPolyline(PolylineOptions option)
        {
            var polyline = NativeMap.AddPolyline(option);
            polyline.Color = option.Color;
            polyline.Width = 8;
            return polyline;
        }

        private void UpdateGoogleMap(Action<MapEx> action)
        {
            if (Control == null)
                return; // this should not occur

            var androidMapView = (MapView)Control;
            if (!androidMapView.IsLaidOut)
                return;

            var formsMap = (MapEx)Element;

            action(formsMap);
        }

        /// <summary>
        /// Move center position
        /// </summary>
        private void UpdateCenter()
        {
            UpdateGoogleMap(formsMap =>
            {
                if (_lastCameraPosition != null && _lastCameraPosition.Target.ToPosition().Equals(formsMap.Center))
                    return; // no change in camera position
                var cameraUpdate = CameraUpdateFactory.NewLatLng(formsMap.Center.ToLatLng());
                NativeMap.MoveCamera(cameraUpdate);
            });
        }

        private void AddMarker(MapPin pin)
        {
            var op = new MarkerOptions();
            op.SetTitle(pin.Label);
            op.SetPosition(pin.Position.ToLatLng());
            op.InvokeIcon(BitmapDescriptorFactory.DefaultMarker(pin.Color.ToAndroidMarkerHue()));
            var marker = NativeMap.AddMarker(op);
            pin.Id = marker.Id;
            Markers.Add(marker);
        }

        private void RemoveMarker(MapPin pin)
        {
            var markerToRemove = Markers.FirstOrDefault(m => m.Id == pin.Id);
            if (markerToRemove == null)
                return;

            markerToRemove.Remove();
            Markers.Remove(markerToRemove);
        }

        private void ClearMarkers()
        {
            // Remove all markers
            foreach (var marker in Markers)
            {
                marker.Remove();
            }
            Markers.Clear();
        }

        private void UpdateMarkers()
        {
            ClearMarkers();
            foreach (var pin in MapView.Pins)
            {
                AddMarker(pin);
            }
        }

        private void UpdateZoomLevel()
        {
            UpdateGoogleMap(formsMap =>
            {
                if (_lastCameraPosition != null && _lastCameraPosition.Zoom.Equals((float)formsMap.ZoomLevel))
                    return; // no change in zoom level

                var cameraUpdate = CameraUpdateFactory.ZoomTo((float)MapView.ZoomLevel);
                NativeMap.MoveCamera(cameraUpdate);
            });
        }

        private void Update()
        {
            UpdateCenter();
            UpdateZoomLevel();
            UpdateRoutes();
            UpdateMarkers();
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);
            if (!_initialized && r > 0 && b > 0)
            {
                Update();
                if (_cameraUpdateInit != null)
                {
                    NativeMap.MoveCamera(_cameraUpdateInit);
                    _cameraUpdateInit = null;
                }
                _initialized = true;
            }
        }
    }
}
