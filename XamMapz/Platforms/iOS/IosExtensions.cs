//
// IosExtensions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;
using UIKit;
using CoreLocation;
using MapKit;
using Microsoft.Maui.Maps;

namespace XamMapz.iOS
{
    /// <summary>
    /// Extension methods for iOS platform
    /// </summary>
    public static class IosExtensions
    {
        public static UIColor ToUIColor(this PinColor color)
        {
            // TODO: support all of the MapPinColors
            switch (color)
            {
                case PinColor.Azure:
                    throw new NotSupportedException();
//                    return UIColor.HueAzure;
                case PinColor.Blue:
                    return UIColor.Blue;
                case PinColor.Cyan:
                    return UIColor.Cyan;
                case PinColor.Green:
                    return UIColor.Green;
                case PinColor.Red:
                    return UIColor.Red;
                case PinColor.Magenta:
                    return UIColor.Magenta;
                case PinColor.Orange:
                    return UIColor.Orange;
                case PinColor.Rose:
                    throw new NotSupportedException();
//                    return UIColor.HueRose;
                case PinColor.Violet:
                    throw new NotSupportedException();
//                    return UIColor.HueViolet;
                case PinColor.Yellow:
                    return UIColor.Yellow;
                default:
                    throw new NotSupportedException(string.Format("Unknown pin color: {0}", color));
            }
        }

        public static UIColor ToUIColor(this Microsoft.Maui.Graphics.Color color)
        {
            return new UIColor((nfloat)color.Red, (nfloat)color.Green, (nfloat)color.Blue, (nfloat)color.Alpha);
        }

        public static CLLocationCoordinate2D ToCoordinate2D(this Location position)
        {
            return new CLLocationCoordinate2D(position.Latitude, position.Longitude);   
        }

        public static MKCoordinateRegion ToMkCoordinateRegion(this MapSpan span)
        {
            return new MKCoordinateRegion(span.Center.ToCoordinate2D(), new MKCoordinateSpan(span.LatitudeDegrees, span.LongitudeDegrees));
        }

        public static Location ToLocation(this CLLocationCoordinate2D coord)
        {
            return new Location(coord.Latitude, coord.Longitude);
        }

        public static MapSpan ToMapSpan(this MKCoordinateRegion region)
        {
            return new MapSpan(region.Center.ToLocation(), region.Span.LatitudeDelta, region.Span.LongitudeDelta);
        }
    }
}

