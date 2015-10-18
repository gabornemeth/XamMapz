using System;
using System.ComponentModel;
using System.Collections.Generic;
using Xamarin.Forms;
using XamMapz.Messaging;
using System.Collections.Specialized;
using Xamarin.Forms.Maps;
using System.Linq;

namespace XamMapz
{
    /// <summary>
    /// Platform independent functionality of custom renderers.
    /// Custom renderer implementations can use it.
    /// </summary>
    public class MapRenderHelper<TPin, TPolyline>
    {
        protected Dictionary<MapPin, TPin> Markers { get; private set; }

        protected Dictionary<MapPolyline, TPolyline> Polylines { get; private set; }

        private IMapRenderer<TPin, TPolyline> _renderer;
        private XamMapz.Map _map;

        public MapRenderHelper(IMapRenderer<TPin, TPolyline> renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");
            _renderer = renderer;
            Markers = new Dictionary<MapPin, TPin>();
            Polylines = new Dictionary<MapPolyline, TPolyline>();
        }

        public void UnbindFromElement()
        {
            if (_map == null)
                return;

            MessagingCenter.Unsubscribe<Map, MapMessage>(this, MapMessage.Message);
//                NativeMap.CameraChange -= NativeMap_CameraChange;
//                NativeMap.MarkerClick -= NativeMap_MarkerClick;
            _map.PinsInternal.CollectionChanged -= OnPinsCollectionChanged;
            _map.PolylinesInternal.CollectionChanged -= OnPolylinesCollectionChanged;
            foreach (var polyline in _map.Polylines)
            {
                polyline.PositionChanged -= polyline_PositionChanged;
                polyline.PropertyChanged -= polyline_PropertyChanged;
            }
            foreach (var pin in _map.Pins)
            {
                pin.PropertyChanged -= pin_PropertyChanged;
            }
            _map = null;
        }

        public void BindToElement(Map map)
        {
            if (map != null)
            {
                _map = map;
//                NativeMap.CameraChange += NativeMap_CameraChange;
//                NativeMap.MarkerClick += NativeMap_MarkerClick;
                map.PinsInternal.CollectionChanged += OnPinsCollectionChanged;
                map.PolylinesInternal.CollectionChanged += OnPolylinesCollectionChanged;
                MessagingCenter.Subscribe<XamMapz.Map, MapMessage>(this, MapMessage.Message, (map1, message) =>
                    {
                        // Handle only messages sent by Element
                        if (map1 != _map)
                            return;

                        _renderer.OnMapMessage(map1, message);
                    });
            }
        }

        public TPolyline GetNativePolyline(MapPolyline polyline)
        {
            if (Polylines.ContainsKey(polyline))
                return Polylines[polyline];

            return default(TPolyline);
        }

        public MapPolyline GetPolyline(TPolyline nativePolyline)
        {
            var itemFound = Polylines.FirstOrDefault(item => item.Value.Equals(nativePolyline));
            if (itemFound.Equals(default(KeyValuePair<MapPolyline, TPolyline>)))
                return null;

            return itemFound.Key;
        }

        #region Pins

        private void OnPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // new pin(s)
                foreach (MapPin pin in e.NewItems)
                {
                    AddMarker(pin);
                    BindPin(pin);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // deleted pin(s)
                foreach (MapPin pin in e.OldItems)
                {
                    RemoveMarker(pin);
                    UnbindPin(pin);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearMarkers();
            }
        }


        private void BindPin(MapPin pin)
        {
            pin.PropertyChanged += pin_PropertyChanged;
        }

        private void UnbindPin(MapPin pin)
        {
            pin.PropertyChanged -= pin_PropertyChanged;
        }

        void pin_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Property of a pin has been changed.
            var pin = sender as MapPin;
            if (pin == null)
                return;

            // retrieve Google Maps marker
            if (Markers.ContainsKey(pin) == false)
                return;

            var nativePin = Markers[pin];
            _renderer.OnPinPropertyChanged(pin, nativePin, e);
        }

        private void AddMarker(MapPin pin)
        {
            var nativePin = _renderer.AddNativePin(pin);
            Markers.Add(pin, nativePin);
        }

        private void RemoveMarker(MapPin pin)
        {
            if (Markers.ContainsKey(pin) == false)
                return;

            var markerToRemove = Markers[pin];

            _renderer.RemoveNativePin(markerToRemove);
            UnbindPin(pin);
            Markers.Remove(pin);
        }

        private void ClearMarkers()
        {
            // Remove all markers
            foreach (var marker in Markers)
            {
                _renderer.RemoveNativePin(marker.Value);
                UnbindPin(marker.Key);
            }
            Markers.Clear();
        }

        public void UpdateMarkers()
        {
            ClearMarkers();
            foreach (var pin in _map.Pins)
            {
                AddMarker(pin);
                BindPin(pin);
            }
        }


        #endregion

        #region Polylines

        private void BindPolyline(MapPolyline polyline)
        {
            polyline.PropertyChanged += polyline_PropertyChanged;
            polyline.PositionChanged += polyline_PositionChanged;
        }

        private void UnbindPolyline(MapPolyline polyline)
        {
            polyline.PropertyChanged -= polyline_PropertyChanged;
            polyline.PositionChanged -= polyline_PositionChanged;
        }

        private void polyline_PositionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var polyline = sender as MapPolyline;
            if (polyline == null)
                return;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // modify the points
                var route = Polylines[polyline];
                if (route != null)
                {
                    var idx = 0;
                    foreach (Position pos in e.NewItems)
                    {
                        _renderer.AddPolylinePosition(route, pos, e.NewStartingIndex + idx++);
                    }
                }
            }
            else
            {
                // rebuild polyline - this is slow
                RemovePolyline(polyline);
                AddPolyline(polyline);
            }
        }

        void polyline_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var polyline = sender as MapPolyline;
            if (polyline == null)
                return;

            var line = Polylines[polyline];
            if (line == null)
                return;

            _renderer.OnPolylinePropertyChanged(polyline, line, e);
        }

        private void OnPolylinesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // new polyline(s)
                foreach (MapPolyline polyline in e.NewItems)
                {
                    AddPolyline(polyline);
                    BindPolyline(polyline);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // deleted polyline(s)
                foreach (MapPolyline polyline in e.OldItems)
                {
                    RemovePolyline(polyline);
                    UnbindPolyline(polyline);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearPolylines();
            }
        }

        public void UpdatePolylines()
        {
            ClearPolylines();

            foreach (var polyline in _map.Polylines)
            {
                AddPolyline(polyline);
                BindPolyline(polyline);
            }
        }

        private void RemovePolyline(MapPolyline polyline)
        {
            if (Polylines.ContainsKey(polyline) == false)
                return;

            RemovePolylineFromMap(polyline);
            Polylines.Remove(polyline);
        }

        private void RemovePolylineFromMap(MapPolyline polyline)
        {
            if (Polylines.ContainsKey(polyline))
            {
                _renderer.RemoveNativePolyline(Polylines[polyline]);
            }
        }

        /// <summary>
        /// Remove all polylines
        /// </summary>
        private void ClearPolylines()
        {
            foreach (var polylineEntry in Polylines)
            {
                UnbindPolyline(polylineEntry.Key);
                RemovePolylineFromMap(polylineEntry.Key);
            }
            Polylines.Clear();
        }

        //        private PolylineOptions CreatePolylineOptions(MapPolyline polyline)
        //        {
        //            var op = new PolylineOptions();
        //            op.InvokeColor(polyline.Color.ToAndroid().ToArgb());
        //            op.InvokeWidth((float)polyline.Width);
        //            op.InvokeZIndex(polyline.ZIndex);
        //            return op;
        //        }

        private void AddPolyline(MapPolyline polyline)
        {
            var nativePolyline = _renderer.AddNativePolyline(polyline);
            Polylines.Add(polyline, nativePolyline);
            // add the new one
//            using (var op = CreatePolylineOptions(polyline))
//            {
//                foreach (var pt in polyline.Positions)
//                {
//                    op.Add(pt);
//                }
//                // add the last polyline segment
//                var line = PolylineAdv.Add(NativeMap, op);
//                _polylines.Add(polyline, line);
//            }
        }

        //        private Polyline AddPolyline(PolylineOptions option)
        //        {
        //            var polyline = NativeMap.AddPolyline(option);
        //            polyline.Color = option.Color;
        //            polyline.Width = option.Width;
        //            return polyline;
        //        }


        #endregion
    }
}

