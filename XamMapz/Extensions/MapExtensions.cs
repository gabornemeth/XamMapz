//
// MapExtensions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using Xamarin.Forms.Maps;

namespace XamMapz.Extensions
{
    /// <summary>
    /// Extension methods for Xamarin Forms Maps
    /// </summary>
    public static class MapExtensions
    {
        public static Position DistanceFrom(this Position pt, Position ptOther)
        {
            return new Position(Math.Abs(ptOther.Latitude - pt.Latitude), Math.Abs(ptOther.Longitude - pt.Longitude));
        }

        public static Position Offset(this Position pos, double latitudeOffset, double longitudeOffset)
        {
            return new Position(pos.Latitude + latitudeOffset, pos.Longitude + longitudeOffset);
        }
    }
}
