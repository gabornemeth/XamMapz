//
// MapX.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2023, Gabor Nemeth
//

using System.Runtime.CompilerServices;
using XamMapz.Handlers;

namespace XamMapz
{
    /// <summary>
    /// Map control based on <see cref="Microsoft.Maui.Controls.Maps.Map"/>
    /// </summary>
    public class MapX : Microsoft.Maui.Controls.Maps.Map
    {
        public event EventHandler<MapViewChangeEventArgs> VisibleRegionChanged;

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == nameof(VisibleRegion))
            {
                VisibleRegionChanged?.Invoke(this, new MapViewChangeEventArgs(VisibleRegion));
            }
            base.OnPropertyChanged(propertyName);
        }

        public Location? Center => VisibleRegion?.Center;

        public Point? ProjectToScreen(Location location)
        {
            return (Handler as IMapXHandler)?.ProjectToScreen(location);
        }
    }
}
