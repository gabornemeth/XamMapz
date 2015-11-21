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
        private MapDictionary<TPin, TPolyline> _dict;
        private IMapRenderer<TPin, TPolyline> _renderer;
        private XamMapz.Map _map;

        public MapRenderHelper(IMapRenderer<TPin, TPolyline> renderer, MapDictionary<TPin, TPolyline> dict)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");
            _renderer = renderer;
            if (dict == null)
                throw new ArgumentNullException("dict");
            _dict = dict;
        }

        public void UnbindFromElement()
        {
            if (_map == null)
                return;

            MessagingCenter.Unsubscribe<Map, MapMessage>(this, MapMessage.Message);
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
            _dict.Clear();
            _map = null;
        }

        public void BindToElement(Map map)
        {
            if (map != null)
            {
                _map = map;
                map.PinsInternal.CollectionChanged += OnPinsCollectionChanged;
                map.PolylinesInternal.CollectionChanged += OnPolylinesCollectionChanged;
                MessagingCenter.Subscribe<XamMapz.Map, MapMessage>(this, MapMessage.Message, (map1, message) =>
                    {
                        // Handle only messages sent by the current map instance
                        if (map1 != _map)
                            return;

                        _renderer.OnMapMessage(map1, message);
                    });
                UpdatePins();
                UpdatePolylines();
            }
        }

        #region Pins

        private void OnPinsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // new pin(s)
                foreach (MapPin pin in e.NewItems)
                {
                    AddPin(pin);
                    BindPin(pin);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                // deleted pin(s)
                foreach (MapPin pin in e.OldItems)
                {
                    RemovePin(pin);
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

            var nativePin = _dict.Pins.GetNative(pin);
            if (nativePin == null)
                return;
            _renderer.OnPinPropertyChanged(pin, nativePin, e);
        }

        private void AddPin(MapPin pin)
        {
            _renderer.AddNativePin(pin);
        }

        private void RemovePin(MapPin pin)
        {
            var pinToRemove = _dict.Pins.GetNative(pin);
            if (pinToRemove == null)
                return;

            _renderer.RemoveNativePin(pinToRemove);
            UnbindPin(pin);
        }

        private void ClearMarkers()
        {
            var pins = _dict.Pins.AsEnumerable().ToArray();
            // Remove all markers
            foreach (var pin in pins)
            {
                _renderer.RemoveNativePin(_dict.Pins.GetNative(pin));
                UnbindPin(pin);
            }
        }

        private void UpdatePins()
        {
            ClearMarkers();
            foreach (var pin in _map.Pins)
            {
                AddPin(pin);
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
                var idx = 0;
                foreach (Position pos in e.NewItems)
                {
                    _renderer.AddPolylinePosition(_dict.Polylines.GetNative(polyline), pos, e.NewStartingIndex + idx++);
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

            var line = _dict.Polylines.GetNative(polyline);
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

        private void UpdatePolylines()
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
            var nativePolyline = _dict.Polylines.GetNative(polyline);
            if (nativePolyline == null)
                return;
            
            _renderer.RemoveNativePolyline(nativePolyline);
        }

        /// <summary>
        /// Remove all polylines
        /// </summary>
        private void ClearPolylines()
        {
            var polylines = _dict.Polylines.AsEnumerable().ToArray(); // Renderer can change the dictionary
            foreach (var polylineEntry in polylines)
            {
                UnbindPolyline(polylineEntry);
                RemovePolyline(polylineEntry);
            }
        }

        /// <summary>
        /// Adds a new instance of <see cref="MapPolyline"/>
        /// </summary>
        /// <param name="polyline">Polyline to add.</param>
        private void AddPolyline(MapPolyline polyline)
        {
            _renderer.AddNativePolyline(polyline);
        }

        #endregion
    }
}

