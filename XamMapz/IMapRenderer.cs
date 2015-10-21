using System;
using System.ComponentModel;
using XamMapz.Messaging;
using Xamarin.Forms.Maps;

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
        void OnMapMessage(XamMapz.Map sender, MapMessage message);

        void AddPolylinePosition(TPolyline nativePolyline, Position position, int index);
    }
}
