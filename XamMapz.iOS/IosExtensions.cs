using System;
using UIKit;
using Xamarin.Forms.Maps;
using CoreLocation;
using MapKit;

namespace XamMapz.iOS
{
    /// <summary>
    /// Extension methods for iOS platform
    /// </summary>
    public static class IosExtensions
    {
        public static UIColor ToUIColor(this MapPinColor color)
        {
            switch (color)
            {
                case MapPinColor.Azure:
                    throw new NotSupportedException();
//                    return UIColor.HueAzure;
                case MapPinColor.Blue:
                    return UIColor.Blue;
                case MapPinColor.Cyan:
                    return UIColor.Cyan;
                case MapPinColor.Green:
                    return UIColor.Green;
                case MapPinColor.Red:
                    return UIColor.Red;
                case MapPinColor.Magenta:
                    return UIColor.Magenta;
                case MapPinColor.Orange:
                    return UIColor.Orange;
                case MapPinColor.Rose:
                    throw new NotSupportedException();
//                    return UIColor.HueRose;
                case MapPinColor.Violet:
                    throw new NotSupportedException();
//                    return UIColor.HueViolet;
                case MapPinColor.Yellow:
                    return UIColor.Yellow;
                default:
                    throw new NotSupportedException(string.Format("Unknown pin color: {0}", color));
            }
        }

        public static UIColor ToUIColor(this Xamarin.Forms.Color color)
        {
            return new UIColor((nfloat)color.R, (nfloat)color.G, (nfloat)color.B, (nfloat)color.A);
        }

        public static CLLocationCoordinate2D ToCoordinate2D(this Position position)
        {
            return new CLLocationCoordinate2D(position.Latitude, position.Longitude);   
        }

        public static MKCoordinateRegion ToMkCoordinateRegion(this MapSpan span)
        {
            return new MKCoordinateRegion(span.Center.ToCoordinate2D(), new MKCoordinateSpan(span.LatitudeDegrees, span.LongitudeDegrees));
        }

        public static Position ToPosition(this CLLocationCoordinate2D coord)
        {
            return new Position(coord.Latitude, coord.Longitude);
        }

        public static MapSpan ToMapSpan(this MKCoordinateRegion region)
        {
            return new MapSpan(region.Center.ToPosition(), region.Span.LatitudeDelta, region.Span.LongitudeDelta);
        }
    }
}

