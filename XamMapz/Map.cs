//
// Map.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XamMapz.Messaging;

namespace XamMapz
{
    /// <summary>
    /// Map control based on <see cref="Xamarin.Forms.Maps.Map"/>
    /// </summary>
    public class Map : Xamarin.Forms.Maps.Map
    {
        private ObservableCollection<MapPin> _pins = new ObservableCollection<MapPin>();

        public new IList<MapPin> Pins
        {
            get { return _pins; }
        }

        internal ObservableCollection<MapPin> PinsInternal
        {
            get { return _pins; }
        }

        public event EventHandler<MapViewChangedEventArgs> ViewChanged;

        public Map()
        {
            MessagingCenter.Subscribe<IMapExRenderer, MapMessage>(this, MapMessage.RendererMessage, (map, message) =>
                {
                    if (message.Map != this)
                        return; // filter by instance
                    OnMapMessage(message);
                });
        }

        ~Map()
        {
            MessagingCenter.Unsubscribe<IMapExRenderer, MapMessage>(this, MapMessage.RendererMessage);
        }

        protected virtual void OnMapMessage(MapMessage message)
        {
            if (message is ViewChangeMessage)
            {
                var msg = (ViewChangeMessage)message;
                Region = msg.Span;
                if (ViewChanged != null)
                    ViewChanged(this, new MapViewChangedEventArgs(msg.Span, msg.ZoomLevel));
                Debug.WriteLine("ViewChangeMessage recieved:\n\tPosition: {0} {1}", msg.Span.Center.Latitude, msg.Span.Center.Longitude);
            }
        }

        public new void MoveToRegion(MapSpan span)
        {
            Region = span;
            MessagingCenter.Send<Map, MapMessage>(this, MapMessage.Message, new ZoomMessage(this, span));
        }

        public Position Center
        {
            get
            {
                return Region.Center;
            }
            set
            {
                MoveToRegion(new MapSpan(value, Region.LatitudeDegrees, Region.LongitudeDegrees));
            }
        }

        public MapSpan Region
        {
            get;
            private set;
        }

        private ObservableCollection<MapPolyline> _polylines = new ObservableCollection<MapPolyline>();

        public IList<MapPolyline> Polylines
        {
            get { return _polylines; }
        }

        internal ObservableCollection<MapPolyline> PolylinesInternal
        {
            get { return _polylines; }
        }

        public Point ProjectToScreen(Position position)
        {
            var msg = new MapProjectMessage(this, position);
            MessagingCenter.Send<XamMapz.Map, MapMessage>(this, MapMessage.Message, msg);
            return msg.ScreenPosition;
        }
    }
}
