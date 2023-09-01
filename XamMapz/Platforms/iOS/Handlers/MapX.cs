using Microsoft.Maui.Maps.Handlers;
using MapKit;
using Microsoft.Maui.Maps.Platform;

namespace XamMapz.Handlers
{
    class MapX : MauiMKMapView
    {
        public MapX(IMapHandler handler) : base(handler)
        {
            GetViewForAnnotation = new MKMapViewAnnotation(GetAnnotationView);
        }

        MKAnnotationView GetAnnotationView(MKMapView mapView, IMKAnnotation annotation)
        {
            if (annotation is IMKAnnotationViewX annotationX)
            {
                return annotationX.View;
            }

            return null;
        }
    }

}