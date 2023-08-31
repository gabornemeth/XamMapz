﻿//
// MapRenderHelper.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System;
using System.ComponentModel;
using System.Collections.Generic;
using XamMapz.Messaging;
using System.Collections.Specialized;

namespace XamMapz
{
    /// <summary>
    /// Platform independent functionality of custom renderers.
    /// Custom renderer implementations can use it.
    /// </summary>
    public class MapRenderHelper<TPin, TPolyline>
    {
        private MapDictionary<TPin, TPolyline> _dict = new MapDictionary<TPin, TPolyline>();
        private IMapRenderer<TPin, TPolyline> _renderer;
        private XamMapz.Map _map;

        public MapRenderHelper(IMapRenderer<TPin, TPolyline> renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException("renderer");
            _renderer = renderer;
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
                ClearPins();
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
            var nativePin = _renderer.AddNativePin(pin);
            _dict.Pins.AddOrUpdate(pin, nativePin);
        }

        private void RemovePin(MapPin pin)
        {
            var pinToRemove = _dict.Pins.GetNative(pin);
            if (pinToRemove == null)
                return;

            _renderer.RemoveNativePin(pinToRemove);
            _dict.Pins.Remove(pin);
            UnbindPin(pin);
        }

        private void ClearPins()
        {
            var pins = _dict.Pins.AsEnumerable().ToArray();
            // Remove all markers
            foreach (var pin in pins)
            {
                RemovePin(pin);
                UnbindPin(pin);
            }
        }

        private void UpdatePins()
        {
            ClearPins();
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
                foreach (Location pos in e.NewItems)
                {
                    var nativePolyline = _dict.Polylines.GetNative(polyline);
                    _renderer.AddPolylineLocation(nativePolyline, pos, e.NewStartingIndex + idx++);
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

            var nativePolyline = _dict.Polylines.GetNative(polyline);
            if (nativePolyline == null)
                return;

            _renderer.OnPolylinePropertyChanged(polyline, ref nativePolyline, e);
            _dict.Polylines.AddOrUpdate(polyline, nativePolyline);
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
            _dict.Polylines.Remove(polyline);
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
            var nativePolyline = _renderer.AddNativePolyline(polyline);
            if (nativePolyline != null)
                _dict.Polylines.AddOrUpdate(polyline, nativePolyline);
        }

        #endregion
    }
}

