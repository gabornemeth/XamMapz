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
using Xamarin.Forms.Maps;

namespace XamMapz.Droid
{
    /// <summary>
    /// Extension methods for easier interop between Xamarin Forms Maps and Android platform
    /// </summary>
    public static class AndroidExtensions
    {
        public static LatLng ToLatLng(this Position pos)
        {
            return new LatLng(pos.Latitude, pos.Longitude);
        }

        public static LatLngBounds ToLatLngBounds(this MapSpan span)
        {
            return new LatLngBounds(new LatLng(span.Center.Latitude - span.LatitudeDegrees * 0.5, span.Center.Longitude - span.LongitudeDegrees * 0.5),
                new LatLng(span.Center.Latitude + span.LatitudeDegrees * 0.5, span.Center.Longitude + span.LongitudeDegrees * 0.5));
        }

        public static Position ToPosition(this LatLng latLng)
        {
            return new Position(latLng.Latitude, latLng.Longitude);
        }

        public static float ToAndroidMarkerHue(this MapPinColor color)
        {
            switch (color)
            {
                case MapPinColor.Azure:
                    return BitmapDescriptorFactory.HueAzure;
                case MapPinColor.Blue:
                    return BitmapDescriptorFactory.HueBlue;
                case MapPinColor.Cyan:
                    return BitmapDescriptorFactory.HueCyan;
                case MapPinColor.Green:
                    return BitmapDescriptorFactory.HueGreen;
                case MapPinColor.Red:
                    return BitmapDescriptorFactory.HueRed;
                case MapPinColor.Magenta:
                    return BitmapDescriptorFactory.HueMagenta;
                case MapPinColor.Orange:
                    return BitmapDescriptorFactory.HueOrange;
                case MapPinColor.Rose:
                    return BitmapDescriptorFactory.HueRose;
                case MapPinColor.Violet:
                    return BitmapDescriptorFactory.HueViolet;
                case MapPinColor.Yellow:
                    return BitmapDescriptorFactory.HueYellow;
                default:
                    throw new NotSupportedException(string.Format("Unknown pin color: {0}", color));
            }
        }
    }
}
