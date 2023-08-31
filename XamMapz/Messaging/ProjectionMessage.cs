namespace XamMapz.Messaging
{
    public class ProjectionMessage : MapMessage
    {
        public Location Location { get; }
        public Point ScreenPosition { get; set; }

        public ProjectionMessage(Map map, Location location) : base(map)
        {
            Location = location;
        }
    }
}
