using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using Xamarin.Forms.Maps;

namespace XamMapz
{
    /// <summary>
    /// Event arguments of position change of a route
    /// </summary>
    public class MapRoutePositionChangeEventArgs : EventArgs
    {
        public NotifyCollectionChangedAction Action { get; private set; }
        public List<Position> Positions { get; private set; }

        public MapRoutePositionChangeEventArgs(NotifyCollectionChangedAction action, List<Position> positions)
        {
            Action = action;
            Positions = positions;
        }
    }
}
