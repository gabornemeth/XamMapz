//
// MapExtensions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2023, Gabor Nemeth
//

namespace XamMapz
{
    /// <summary>
    /// Extension methods for Maui Maps
    /// </summary>
    public static class MapExtensions
    {
        public static Location DistanceFrom(this Location loc, Location locOther)
        {
            return new Location(Math.Abs(locOther.Latitude - loc.Latitude), Math.Abs(locOther.Longitude - loc.Longitude));
        }

        public static Location Offset(this Location loc, double latitudeOffset, double longitudeOffset)
        {
            return new Location(loc.Latitude + latitudeOffset, loc.Longitude + longitudeOffset);
        }

        public static MauiAppBuilder UseXamMapz(this MauiAppBuilder app) =>
            app.UseMauiMaps()
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(MapX), typeof(XamMapz.Handlers.MapXHandler));
                handlers.AddHandler(typeof(PinX), typeof(XamMapz.Handlers.PinXHandler));
                handlers.AddHandler(typeof(PolylineX), typeof(XamMapz.Handlers.PolylineXHandler));
            });
    }
}
