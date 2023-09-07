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
        public Location Target { get; private set; }

        public MoveMessage(MapX map, Location target) : base(map)
        {
            Target = target;
        }
    }
}

