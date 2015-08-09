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
                    Center = msg.Span.Center;
                    ZoomLevel = msg.ZoomLevel;
                    Region = msg.Span;
                    if (ViewChanged != null)
                        ViewChanged(this, new MapViewChangedEventArgs(msg.Span, msg.ZoomLevel));
                }
            });
        }

        ~MapEx()
        {
            MessagingCenter.Unsubscribe<IMapExRenderer, MapMessage>(this, MapMessage.Message);
        }

        /// <summary>
        /// Zooms to the given <see cref="MapSpan"/>
        /// </summary>
        /// <param name="bounds">Bound to zoom</param>
        public void ZoomTo(MapSpan bounds)
        {
            MessagingCenter.Send<MapEx, MapMessage>(this, MapMessage.Message, new ZoomMessage(bounds));
        }

        #region ZoomLevel dependency property

        public static readonly BindableProperty ZoomLevelProperty = BindableProperty.Create<MapEx, double>(map => map.ZoomLevel, 0);

        public double ZoomLevel
        {
            get
            {
                return (double)GetValue(ZoomLevelProperty);
            }
            set
            {
                SetValue(ZoomLevelProperty, value);
            }
        }

        #endregion

        #region Center dependency property

        public static readonly BindableProperty CenterProperty = BindableProperty.Create<MapEx, Position>(map => map.Center, new Position(0, 0));

        public Position Center
        {
            get
            {
                return (Position)GetValue(CenterProperty);
            }
            set
            {
                SetValue(CenterProperty, value);
            }
        }

        #endregion

        #region Region dependency property

        public MapSpan Region
        {
            get; private set;
        }

        #endregion

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
