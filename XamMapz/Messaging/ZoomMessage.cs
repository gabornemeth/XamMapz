//
// ZoomMessage.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;
using Xamarin.Forms.Maps;

namespace XamMapz.Messaging
{
    public class ZoomMessage : MapMessage
    {
        public MapSpan Bounds { get; private set; }

        public ZoomMessage(Map map, MapSpan bounds) : base(map)
        {
            Bounds = bounds;
        }
    }
}
