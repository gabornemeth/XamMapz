//
// MapHandler.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2023, Gabor Nemeth
//

using Microsoft.Maui.Maps.Platform;

namespace XamMapz.Handlers
{
    partial class MapHandler
    {
        protected override MauiMKMapView CreatePlatformView()
        {
            return new MapX(this);
        }
    }
}
