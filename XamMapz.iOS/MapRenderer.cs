using System;
using Xamarin.Forms;
using MapKit;
using Map = XamMapz.Map;
using XamMapz.Messaging;
using System.Linq;
using UIKit;
using System.Collections.Generic;

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
        private Dictionary<MKPolyline, MapPolyline> _polylines = new Dictionary<MKPolyline, MapPolyline>();

        protected MKMapView NativeMap
        {
            get
            {
                return Control as MKMapView;
            }
        }

        public MapRenderer()
        {
            _renderHelper = new MapRenderHelper<MKPointAnnotation, MKPolyline>(this);
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
                Bind(e.NewElement as Map);
                UpdateRegion();
                _renderHelper.UpdateMarkers();
                _renderHelper.UpdatePolylines();
            }
        }

        private void Unbind()
        {
            _renderHelper.UnbindFromElement();
            NativeMap.Delegate = null;
        }

        private void Bind(Map map)
        {
            try
            {
                _renderHelper.BindToElement(map);
                // overwriting existing delegate throws exception, so set it to null first
                NativeMap.Delegate = null;
                NativeMap.Delegate = new MapViewDelegate(this);
            }
            catch (Exception ex)
            {
                var x = ex;
            }
        }

        private class MapViewDelegate : MKMapViewDelegate
        {
            private MapRenderer _renderer;

            public MapViewDelegate(MapRenderer renderer)
            {
                _renderer = renderer;
            }

            public override MKOverlayView GetViewForOverlay(MKMapView mapView, IMKOverlay overlay)
            {
                if (overlay is MKPolyline)
                {
                    var overlayPolyline = (MKPolyline)overlay;
                    var polyline = _renderer._polylines[overlayPolyline];
                    var polylineView = new MKPolylineView(overlayPolyline);
                    polylineView.StrokeColor = polyline.Color.ToUIColor();
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
                    //                MKAnnotationView pinView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation (pId);

                    //                if (pinView == null)
                    var pinView = new MKPinAnnotationView(annotation, "myPinId");

                    //                pinView.PinColor = annotationPoint..Red;
                    pinView.CanShowCallout = true;

                    return pinView;
                }
                //            if (annotation is MKPolyline)
                //            {
                //                var annotationPolyline = (MKPolyline)annotation;
                //                var polylineView = new mkan(annotationPolyline);
                //
                //                return polylineView;
                //            }

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
//            var camera = NativeMap.Camera.Copy() as MKMapCamera;
//            camera.CenterCoordinate = Map.Region.Center.ToCoordinate2D();
            NativeMap.Region = Map.Region.ToMkCoordinateRegion();
//            NativeMap.SetCamera(camera, false);
        }

        #region IMapRenderer implementation

        public void OnMapMessage(Map sender, XamMapz.Messaging.MapMessage message)
        {
            if (message is ZoomMessage)
            {
//                var msg = (ZoomMessage)message;
                UpdateRegion();
            }
            else if (message is MapProjectMessage)
            {
                var msg = (MapProjectMessage)message;
                var screenPos = MKMapPoint.FromCoordinate(msg.Position.ToCoordinate2D());
                msg.ScreenPosition = new Point(screenPos.X, screenPos.Y);
            }
        }

        public void AddPolylinePosition(MKPolyline nativePolyline, Xamarin.Forms.Maps.Position position, int index)
        {
            var view = NativeMap.GetViewForOverlay(NativeMap, nativePolyline) as MKPolylineView;
//            using (var latlng = pos.ToLatLng())
//            {
//                route.Add(latlng);
//            }

        }

        public void OnPinPropertyChanged(MapPin pin, MKPointAnnotation nativePin, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == MapPin.ColorProperty.PropertyName)
            {
                // Update pin's color
                var nativeView = NativeMap.ViewForAnnotation(nativePin) as MKPinAnnotationView;
                if (nativeView != null)
                {
                    nativeView.PinTintColor = pin.Color.ToUIColor();
                }
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
            if (nativeView == null)
                return;
            
            if (e.PropertyName == MapPolyline.ColorProperty.PropertyName)
            {
                // change color of the polyline
                nativeView.TintColor = polyline.Color.ToUIColor();
            }
            else if (e.PropertyName == MapPolyline.ZIndexProperty.PropertyName)
            {
                // change Z-index of the polyline
                //line.ZIndex = polyline.ZIndex;
            }
            else if (e.PropertyName == MapPolyline.PositionsProperty)
            {
                RemoveNativePolyline(nativePolyline);
                AddNativePolyline(polyline);
            }
        }

        public void RemoveNativePin(MKPointAnnotation nativePin)
        {
            NativeMap.RemoveAnnotation(nativePin);
            nativePin.Dispose();
        }

        public MKPointAnnotation AddNativePin(MapPin pin)
        {
            var nativePin = new MKPointAnnotation
            {
                Title = pin.Label,
                Coordinate = pin.Position.ToCoordinate2D()
            };
                
            NativeMap.AddAnnotation(nativePin);
            return nativePin;
        }

        public MKPolyline AddNativePolyline(MapPolyline polyline)
        {
            var coords = from pos in polyline.Positions
                                  select pos.ToCoordinate2D();
            var nativePolyline = MKPolyline.FromCoordinates(coords.ToArray());
            _polylines.Add(nativePolyline, polyline);
            NativeMap.AddOverlay(nativePolyline);
            return nativePolyline;
        }

        public void RemoveNativePolyline(MKPolyline nativePolyline)
        {
            // remove the old polyline from the map
            NativeMap.RemoveAnnotation(nativePolyline);
            nativePolyline.Dispose();
            _polylines.Remove(nativePolyline);
        }

        #endregion

    }
}

