using System;
using Xamarin.Forms.Maps;

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
