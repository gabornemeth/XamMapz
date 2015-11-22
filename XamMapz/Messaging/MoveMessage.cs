using System;
using Xamarin.Forms.Maps;

namespace XamMapz.Messaging
{
    /// <summary>
    /// Message about repositioning the center of the view
    /// </summary>
    public class MoveMessage : MapMessage
    {
        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public Position Target { get; private set; }

        public MoveMessage(Map map, Position target) : base(map)
        {
            Target = target;
        }
    }
}

