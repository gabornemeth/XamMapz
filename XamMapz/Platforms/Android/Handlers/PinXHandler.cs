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
                if (mapPin.Color == XamMapz.PinColor.Default)
                {
                    handler.PlatformView.SetIcon(null);
                }
                else
                {
                    handler.PlatformView.SetIcon(BitmapDescriptorFactory.DefaultMarker(mapPin.Color.ToAndroidMarkerHue()));
                }
            }
        }
    }
}
