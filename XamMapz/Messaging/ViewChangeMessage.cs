//
// MapExtensions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2023, Gabor Nemeth
//

using Microsoft.Maui.Maps;

namespace XamMapz.Messaging
{
    /// <summary>
    /// Message about view change
    /// </summary>
    public class ViewChangeMessage : MapMessage
    {
        public MapSpan Span { get; set; }
        public float ZoomLevel { get; set; }

        public ViewChangeMessage(Map map) : base(map)
        {
        }
    }
}
