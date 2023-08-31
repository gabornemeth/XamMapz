//
// MapExtensions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2023, Gabor Nemeth
//

using Microsoft.Maui.Maps.Handlers;
using System.Runtime.CompilerServices;

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

        public static MauiAppBuilder UseXamMapz(this MauiAppBuilder app) =>
            app.UseMauiMaps()
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(MapPin), typeof(MapPinHandler));
            });
    }
}
