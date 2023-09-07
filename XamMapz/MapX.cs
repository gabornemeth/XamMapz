//
// MapX.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2023, Gabor Nemeth
//

using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using System.Diagnostics;
using XamMapz.Messaging;
using System.Runtime.CompilerServices;

namespace XamMapz
{
    /// <summary>
    /// Map control based on <see cref="Microsoft.Maui.Controls.Maps.Map"/>
    /// </summary>
    public class MapX : Microsoft.Maui.Controls.Maps.Map
    {
        public event EventHandler<MapViewChangeEventArgs> ViewChanged;

        //public MapX()
        //{
        //    MessagingCenter.Subscribe<IMapRenderer, MapMessage>(this, MapMessage.RendererMessage, (map, message) =>
        //        {
        //            if (message.Map != this)
        //                return; // filter by instance
        //            OnMapMessage(message);
        //        });
        //}

        //~MapX()
        //{
        //    MessagingCenter.Unsubscribe<IMapRenderer, MapMessage>(this, MapMessage.RendererMessage);
        //}

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == nameof(VisibleRegion))
            {
                ViewChanged?.Invoke(this, new MapViewChangeEventArgs(VisibleRegion, 0));
            }
            base.OnPropertyChanged(propertyName);
        }

        protected virtual void OnMapMessage(MapMessage message)
        {
            if (message is ViewChangeMessage)
            {
                var msg = (ViewChangeMessage)message;
                Region = msg.Span;
                if (ViewChanged != null)
                    ViewChanged(this, new MapViewChangeEventArgs(msg.Span, msg.ZoomLevel));
                Debug.WriteLine("ViewChangeMessage recieved:\n\tPosition: {0} {1}", msg.Span.Center.Latitude, msg.Span.Center.Longitude);
            }
        }

        public Location Center
        {
            get
            {
                return Region?.Center ?? new Location();
            }
            //set
            //{
            //    MessagingCenter.Send<Map, MapMessage>(this, MapMessage.Message, new MoveMessage(this, value));
            //}
        }

        public BindableProperty RegionProperty = BindableProperty.Create(nameof(Region), typeof(MapSpan), typeof(MapX), propertyChanged: OnRegionChanged);

        public MapSpan Region
        {
            get
            {
                return GetValue(RegionProperty) as MapSpan;
            }
            private set
            {
                SetValue(RegionProperty, value);
            }
        }

        private static void OnRegionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue == newValue)
                return;

            var map = (MapX)bindable;
            map.OnPropertyChanged(nameof(Center));
        }


        private ObservableCollection<PolylineX> _polylines = new ObservableCollection<PolylineX>();

        public IList<PolylineX> Polylines
        {
            get { return _polylines; }
        }

        internal ObservableCollection<PolylineX> PolylinesInternal
        {
            get { return _polylines; }
        }

        public Point ProjectToScreen(Location location)
        {
            var msg = new ProjectionMessage(this, location);
            MessagingCenter.Send<XamMapz.MapX, MapMessage>(this, MapMessage.Message, msg);
            return msg.ScreenPosition;
        }
    }
}
