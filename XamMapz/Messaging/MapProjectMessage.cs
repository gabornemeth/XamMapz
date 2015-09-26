using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XamMapz.Messaging;

namespace XamMapz.Messaging
{
    public class MapProjectMessage : MapMessage
    {
        public Position Position { get; private set; }
        public Point ScreenPosition { get; set; }

        public MapProjectMessage(Map map, Position position) : base(map)
        {
            Position = position;
        }
    }
}
