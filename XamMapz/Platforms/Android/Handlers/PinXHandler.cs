//
// MapPinHandler.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2023, Gabor Nemeth
//

using Android.Gms.Maps.Model;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps;
using XamMapz.Droid;
using Android.Gms.Maps;

namespace XamMapz.Handlers
{
    public partial class PinXHandler
    {
        static PinXHandler()
        {
            Mapper.Add(PinX.ColorProperty.PropertyName, MapPinColor);
        }

        public PinXHandler() : base(Mapper)
        {
        }

        public static void MapPinColor(IMapPinHandler handler, IMapPin pin)
        {
            if (pin is PinX mapPin)
            {
                var marker = mapPin.MapHandler.GetNativeMarker(pin.MarkerId);

                if (mapPin.Color == PinColor.Default)
                {
                    marker?.SetIcon(null);
                    handler.PlatformView.SetIcon(null);
                }
                else
                {
                    marker?.SetIcon(BitmapDescriptorFactory.DefaultMarker(mapPin.Color.ToAndroidMarkerHue()));
                    handler.PlatformView.SetIcon(BitmapDescriptorFactory.DefaultMarker(mapPin.Color.ToAndroidMarkerHue()));
                }
            }
        }
    }
}
