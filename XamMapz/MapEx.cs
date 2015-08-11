//
// MapEx.cs
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
    public class MapEx : Map
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

        public MapEx()
        {
            MessagingCenter.Subscribe<IMapExRenderer, MapMessage>(this, MapMessage.RendererMessage, (map, message) =>
            {
                Debug.WriteLine("Map renderer message recieved: {0}", message);
                if (message is ViewChangeMessage)
                {
                    var msg = (ViewChangeMessage)message;
                    MoveToRegion(msg.Span);
                    //Center = msg.Span.Center;
                    //ZoomLevel = msg.ZoomLevel;
                    //Region = msg.Span;
                    if (ViewChanged != null)
                        ViewChanged(this, new MapViewChangedEventArgs(msg.Span, msg.ZoomLevel));
                }
            });
        }

        ~MapEx()
        {
            MessagingCenter.Unsubscribe<IMapExRenderer, MapMessage>(this, MapMessage.Message);
        }

        public new void MoveToRegion(MapSpan span)
        {
            Region = span;
            base.MoveToRegion(span);
        }

        public Position Center
        {
            get
            {
                return Region.Center;
            }
            set
            {
                Region = new MapSpan(value, Region.LatitudeDegrees, Region.LongitudeDegrees);
            }
        }

        public MapSpan Region
        {
            get; private set;
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
    }
}
