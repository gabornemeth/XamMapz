//
// IMapRenderer.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//
        
using System.ComponentModel;
using XamMapz.Messaging;

namespace XamMapz
{
    public interface IMapRenderer
    {
    }

    /// <summary>
    /// Interface has to be implemented by platform specific custom renderer
    /// </summary>
    public interface IMapRenderer<TPin, TPolyline> : IMapRenderer
    {
        void OnPinPropertyChanged(MapPin pin, TPin nativePin, PropertyChangedEventArgs e);
        void OnPolylinePropertyChanged(MapPolyline polyline, ref TPolyline nativePolyline, PropertyChangedEventArgs e);
        void RemoveNativePin(TPin nativePin);
        TPin AddNativePin(MapPin pin);

        TPolyline AddNativePolyline(MapPolyline polyline);
        void RemoveNativePolyline(TPolyline polyline);

        /// <summary>
        /// Message received from the portable map control
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="message">Message.</param>
        void OnMapMessage(XamMapz.MapX sender, MapMessage message);

        void AddPolylineLocation(TPolyline nativePolyline, Location location, int index);
    }
}
