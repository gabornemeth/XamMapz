//
// AndroidExtensions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;
using Android.Gms.Maps.Model;
using Microsoft.Maui.Maps;

namespace XamMapz.Droid
{
    /// <summary>
    /// Extension methods for easier interop between Xamarin Forms Maps and Android platform
    /// </summary>
    public static class AndroidExtensions
    {
        public static LatLng ToLatLng(this Location pos)
        {
            return new LatLng(pos.Latitude, pos.Longitude);
        }

        public static LatLngBounds ToLatLngBounds(this MapSpan span)
        {
            return new LatLngBounds(new LatLng(span.Center.Latitude - span.LatitudeDegrees * 0.5, span.Center.Longitude - span.LongitudeDegrees * 0.5),
                new LatLng(span.Center.Latitude + span.LatitudeDegrees * 0.5, span.Center.Longitude + span.LongitudeDegrees * 0.5));
        }

        public static Location ToLocation(this LatLng latLng)
        {
            return new Location(latLng.Latitude, latLng.Longitude);
        }

        public static float ToAndroidMarkerHue(this PinColor color)
        {
            switch (color)
            {
                case PinColor.Azure:
                    return BitmapDescriptorFactory.HueAzure;
                case PinColor.Blue:
                    return BitmapDescriptorFactory.HueBlue;
                case PinColor.Cyan:
                    return BitmapDescriptorFactory.HueCyan;
                case PinColor.Green:
                    return BitmapDescriptorFactory.HueGreen;
                case PinColor.Red:
                    return BitmapDescriptorFactory.HueRed;
                case PinColor.Magenta:
                    return BitmapDescriptorFactory.HueMagenta;
                case PinColor.Orange:
                    return BitmapDescriptorFactory.HueOrange;
                case PinColor.Rose:
                    return BitmapDescriptorFactory.HueRose;
                case PinColor.Violet:
                    return BitmapDescriptorFactory.HueViolet;
                case PinColor.Yellow:
                    return BitmapDescriptorFactory.HueYellow;
                default:
                    throw new NotSupportedException(string.Format("Unknown pin color: {0}", color));
            }
        }

        public static void Add(this PolylineOptions options, Location location)
        {
            using (var latlng = location.ToLatLng())
            {
                options.Add(latlng);
            }
        }
    }
}
