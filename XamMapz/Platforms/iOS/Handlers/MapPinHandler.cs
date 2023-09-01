//
// MapPinHandler.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2023, Gabor Nemeth
//

using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps;
using XamMapz.iOS;
using MapKit;
using UIKit;

namespace XamMapz.Handlers
{
    public partial class MapPinHandler
    {
        static MapPinHandler()
        {
            Mapper.Add(MapPin.ColorProperty.PropertyName, MapPinColor);
        }

        public MapPinHandler() : base(Mapper)
        {
        }

        protected override IMKAnnotation CreatePlatformElement()
        {
            if (OperatingSystem.IsIOSVersionAtLeast(11))
            {
                return new MKMarkerAnnotationViewX();
            }

            return new MKPinAnnotationViewX();
        }

        public static void MapPinColor(IMapPinHandler handler, IMapPin pin)
        {
            if (pin is MapPin mapPin)
            {
                if (mapPin.Color == XamMapz.MapPinColor.Default)
                {
                }
                else
                {
                    var platformView = (IMKAnnotationViewX)handler.PlatformView;
                    platformView.SetColor(mapPin.Color.ToUIColor());
                }
            }
        }
    }

    interface IMKAnnotationViewX
    {
        MKAnnotationView View { get; }
        void SetColor(UIColor color);
    }

    abstract class MKAnnotationViewX<TView> : MKPointAnnotation, IMKAnnotationViewX
        where TView : MKAnnotationView
    {
        public TView View { get; }

        MKAnnotationView IMKAnnotationViewX.View => View;

        protected MKAnnotationViewX()
        {
            View = CreateView("myPinId");
            if (View == null)
            {
                throw new Exception("CreateView returned null.");
            }

            View.CanShowCallout = true;
        }

        protected abstract TView CreateView(string id);

        public abstract void SetColor(UIColor color);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                View?.Dispose();
            }
        }
    }

    class MKPinAnnotationViewX : MKAnnotationViewX<MKPinAnnotationView>
    {
        protected override MKPinAnnotationView CreateView(string id)
        {
            return new MKPinAnnotationView(this, id);
        }

        public override void SetColor(UIColor color)
        {
            View.PinTintColor = color;
        }
    }

    class MKMarkerAnnotationViewX : MKAnnotationViewX<MKMarkerAnnotationView>
    {
        protected override MKMarkerAnnotationView CreateView(string id)
        {
            return new MKMarkerAnnotationView(this, id);
        }

        public override void SetColor(UIColor color)
        {
            View.MarkerTintColor = color;
        }
    }
}