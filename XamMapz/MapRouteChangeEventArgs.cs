using System;
using System.Collections.Generic;
using System.Text;

namespace XamMapz
{
    public enum MapRouteChangeAction
    {
        /// <summary>
        /// New route has been added
        /// </summary>
        Add,
        /// <summary>
        /// Route has been removed
        /// </summary>
        Remove
    }

    public class MapRouteChangeEventArgs : EventArgs
    {
        public MapRouteChangeAction Action { get; private set; }
        public MapPolyline Route { get; private set; }

        public MapRouteChangeEventArgs(MapRouteChangeAction action, MapPolyline route)
        {
            Action = action;
            Route = route;
        }
    }
}
