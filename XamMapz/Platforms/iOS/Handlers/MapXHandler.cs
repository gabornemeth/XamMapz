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
    partial class MapXHandler : Microsoft.Maui.Maps.Handlers.MapHandler, IMapXHandler
    {
        protected override MauiMKMapView CreatePlatformView()
        {
            return new MKMapViewX(this);
        }

        public Point? ProjectToScreen(Location location)
        {
            throw new NotImplementedException();
        }
    }
}
