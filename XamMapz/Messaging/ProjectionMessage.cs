using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XamMapz.Messaging;

namespace XamMapz.Messaging
{
    public class ProjectionMessage : MapMessage
    {
        public Position Position { get; private set; }
        public Point ScreenPosition { get; set; }

        public ProjectionMessage(Map map, Position position) : base(map)
        {
            Position = position;
        }
    }
}
