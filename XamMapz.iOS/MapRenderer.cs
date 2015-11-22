//
// MapRenderer.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using Xamarin.Forms;
using MapKit;
using Map = XamMapz.Map;
using XamMapz.Messaging;
using System.Linq;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Xamarin.Forms.Platform.iOS;
using CoreLocation;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(XamMapz.Map), typeof(XamMapz.iOS.MapRenderer))]
namespace XamMapz.iOS
{
    /// <summary>
    /// Map renderer for iOS - based on Xamarin Forms' MapRenderer
    /// Using MapKit
    /// </summary>
    public class MapRenderer : Xamarin.Forms.Maps.iOS.MapRenderer, IMapRenderer<MKPointAnnotation, MKPolyline>
    {
        private MapRenderHelper<MKPointAnnotation, MKPolyline> _renderHelper;
        private MapDictionary<MKPointAnnotation, MKPolyline> _dictionary = new MapDictionary<MKPointAnnotation, MKPolyline>();
        private MapViewDelegate _mapViewDelegate;

        protected MKMapView NativeMap
        {
            get
            {
                return Control as MKMapView;
            }
        }

        public MapRenderer()
        {
            _renderHelper = new MapRenderHelper<MKPointAnnotation, MKPolyline>(this, _dictionary);
            _mapViewDelegate = new MapViewDelegate(this);
        }

        private bool _isDisposed;

        protected override void Dispose(bool disposing)
        {
            if (disposing & !_isDisposed)
            {
                Unbind();
                _isDisposed = true;
            }
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
                Unbind();
            if (e.NewElement != null)
            {
//                if (Control == null)
//                {
//                    SetNativeControl(new MKMapView(CGRect.Empty));
//                }
                Bind(e.NewElement as Map);
                UpdateRegion();
//                UpdateIsShowingUser();
            }
        }

        //        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //        {
        //            if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
        //            {
        //                UpdateIsShowingUser();
        //            }
        //            else
        //                base.OnElementPropertyChanged(sender, e);
        //        }

        private CLLocationManager _locationManager;

        private void UpdateIsShowingUser()
        {
            Debug.WriteLine("ShowsUserLocation before: {0}", NativeMap.ShowsUserLocation);
            if (Map.IsShowingUser && UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                _locationManager = new CLLocationManager();
                _locationManager.RequestWhenInUseAuthorization(); // only in foreground
            }
            NativeMap.ShowsUserLocation = Map.IsShowingUser;
            Debug.WriteLine("ShowsUserLocation after: {0}", NativeMap.ShowsUserLocation);
        }

        private void Unbind()
        {
            _renderHelper.UnbindFromElement();
            NativeMap.Delegate = null;
            _dictionary.Clear();
        }

        private void Bind(Map map)
        {
            try
            {
                _renderHelper.BindToElement(map);
                // overwriting existing delegate throws exception, so set it to null first
                NativeMap.Delegate = null;
                NativeMap.Delegate = _mapViewDelegate;
            }
            catch (Exception ex)
            {
                var x = ex;
            }
        }

        /// <summary>
        /// Custom <see cref="MKMapViewDelegate"/> for handling events of the map control.
        /// </summary>
        private class MapViewDelegate : MKMapViewDelegate
        {
            private MapRenderer _renderer;

            public MapViewDelegate(MapRenderer renderer)
            {
                _renderer = renderer;
            }

            public bool IsRegionChanging { get; private set; }

            public override void RegionWillChange(MKMapView mapView, bool animated)
            {
                Debug.WriteLine("RegionWillChange");
                if (IsRegionChanging)
                    return;
                IsRegionChanging = true;
            }

            public override void RegionChanged(MKMapView mapView, bool animated)
            {
                Debug.WriteLine("RegionChanged");
                MessagingCenter.Send<IMapRenderer, MapMessage>(_renderer, MapMessage.RendererMessage,
                    new ViewChangeMessage(_renderer.Map) { Span = mapView.Region.ToMapSpan() });
                IsRegionChanging = false;
            }

            public override void DidUpdateUserLocation(MKMapView mapView, MKUserLocation userLocation)
            {
                Debug.WriteLine("DidUpdateUserLocation");
            }

            public override void DidStopLocatingUser(MKMapView mapView)
            {
                Debug.WriteLine("DidStopLocatingUser");
            }

            public override void WillStartLocatingUser(MKMapView mapView)
            {
                Debug.WriteLine("WillStartLocatingUser. ShowsUserLocation = {0}", mapView.ShowsUserLocation);
            }

            public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
            {
                if (view is MKPinAnnotationView)
                {
                    // clicked on a pin
                    var pin = _renderer._dictionary.Pins.Get(view.Annotation as MKPointAnnotation);
                    if (pin == null)
                        return;

                    pin.OnClicked();
                }
            }

            public override MKOverlayView GetViewForOverlay(MKMapView mapView, IMKOverlay overlay)
            {
                if (overlay is MKPolyline)
                {
                    var overlayPolyline = (MKPolyline)overlay;
                    var polyline = _renderer._dictionary.Polylines.Get(overlayPolyline);
                    var polylineView = new MKPolylineView(overlayPolyline);
                    _renderer.UpdatePolylineColor(polyline, polylineView);
                    return polylineView;
                }

                return null;
            }

            public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
            {
                if (annotation is MKPointAnnotation)
                {
                    var annotationPoint = (MKPointAnnotation)annotation;
                    // create pin annotation view
                    var pinView = new MKPinAnnotationView(annotation, "myPinId");
                    _renderer.UpdatePinColor(_renderer._dictionary.Pins.Get(annotationPoint), pinView);
                    pinView.CanShowCallout = true;

                    return pinView;
                }

                return null;
            }
        }

        private Map Map
        {
            get
            {
                return Element as Map;
            }
        }

        private void UpdateRegion()
        {
            NativeMap.Region = Map.Region.ToMkCoordinateRegion();
        }

        private void UpdatePinColor(MapPin pin, MKPinAnnotationView nativeView)
        {
            if (nativeView != null)
            {
                nativeView.PinTintColor = pin.Color.ToUIColor();
            }
        }

        private void UpdatePolylineColor(MapPolyline polyline, MKPolylineView nativeView)
        {
            if (nativeView != null)
            {
                nativeView.StrokeColor = polyline.Color.ToUIColor();
            }
        }


        #region IMapRenderer implementation

        public void OnMapMessage(Map sender, XamMapz.Messaging.MapMessage message)
        {
            if (message is ZoomMessage)
            {
//                var msg = (ZoomMessage)message;
                if (_mapViewDelegate.IsRegionChanging == false)
                    UpdateRegion();
            }
            else if (message is ProjectionMessage)
            {
                var msg = (ProjectionMessage)message;
                var screenPos = MKMapPoint.FromCoordinate(msg.Position.ToCoordinate2D());
                msg.ScreenPosition = new Point(screenPos.X, screenPos.Y);
            }
            else if (message is MoveMessage)
            {
                if (_mapViewDelegate.IsRegionChanging == false)
                {
                    var msg = (MoveMessage)message;
                    NativeMap.CenterCoordinate = msg.Target.ToCoordinate2D();
                }
            }
        }

        public void AddPolylinePosition(MKPolyline nativePolyline, Xamarin.Forms.Maps.Position position, int index)
        {
            var view = NativeMap.ViewForOverlay(nativePolyline) as MKPolylineView;
//            if (view == null)
            {
                // no position yet
                var polyline = _dictionary.Polylines.Get(nativePolyline);
                RemoveNativePolyline(nativePolyline);

                AddNativePolyline(polyline);
            }
        }

        public void OnPinPropertyChanged(MapPin pin, MKPointAnnotation nativePin, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MapPin.ColorProperty.PropertyName)
            {
                // Update pin's color
                var nativeView = NativeMap.ViewForAnnotation(nativePin) as MKPinAnnotationView;
                UpdatePinColor(pin, nativeView);
            }
            else if (e.PropertyName == MapPin.PositionProperty.PropertyName)
            {
                // Update pin's position
                nativePin.SetCoordinate(pin.Position.ToCoordinate2D());
            }
        }

        public void OnPolylinePropertyChanged(MapPolyline polyline, MKPolyline nativePolyline, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var nativeView = NativeMap.ViewForOverlay(nativePolyline) as MKPolylineView;
            
            if (e.PropertyName == MapPolyline.ColorProperty.PropertyName)
            {
                // change color of the polyline
                if (nativeView != null)
                    UpdatePolylineColor(polyline, nativeView);
            }
            else if (e.PropertyName == MapPolyline.ZIndexProperty.PropertyName)
            {
                // change Z-index of the polyline
                // we need to readd the polyline
                RemoveNativePolyline(nativePolyline);
                nativePolyline = AddNativePolyline(polyline);
            }
            else if (e.PropertyName == MapPolyline.PositionsProperty)
            {
                RemoveNativePolyline(nativePolyline);
                nativePolyline = AddNativePolyline(polyline);
            }
        }

        public void RemoveNativePin(MKPointAnnotation nativePin)
        {
            NativeMap.RemoveAnnotation(nativePin);
//            nativePin.Dispose();
            _dictionary.Pins.Remove(nativePin);
        }

        public MKPointAnnotation AddNativePin(MapPin pin)
        {
            var nativePin = new MKPointAnnotation
            {
                Title = pin.Label,
                Coordinate = pin.Position.ToCoordinate2D()
            };
            _dictionary.Pins.AddOrUpdate(pin, nativePin);    
            NativeMap.AddAnnotation(nativePin);
            return nativePin;
        }

        public MKPolyline AddNativePolyline(MapPolyline polyline)
        {
            var coords = from pos in polyline.Positions
                                  select pos.ToCoordinate2D();
            var nativePolyline = MKPolyline.FromCoordinates(coords.ToArray());
            // Linear search the place of the polyline
            int i = 0;
            if (NativeMap.Overlays != null)
            {
                for (i = 0; i < NativeMap.Overlays.Length; i++)
                {
                    if (NativeMap.Overlays[i] is MKPolyline)
                    {
                        var p = _dictionary.Polylines.Get((MKPolyline)NativeMap.Overlays[i]);
                        if (p == null)
                            continue;
                        if (p.ZIndex > polyline.ZIndex)
                        {
                            break;
                        }
                    }
                }
            }
            _dictionary.Polylines.AddOrUpdate(polyline, nativePolyline);
            NativeMap.InsertOverlay(nativePolyline, i);
            Debug.WriteLine(string.Format("Native polyline {0:x} added.", nativePolyline.GetHashCode()));
            return nativePolyline;
        }

        public void RemoveNativePolyline(MKPolyline nativePolyline)
        {
            // remove the old polyline from the map
            NativeMap.RemoveOverlay(nativePolyline);
//            nativePolyline.Dispose();
            _dictionary.Polylines.Remove(nativePolyline);
            Debug.WriteLine(string.Format("Native polyline {0:x} removed.", nativePolyline.GetHashCode()));
        }

        #endregion

    }
}

