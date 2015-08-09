//
// MapPin.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace XamMapz
{
    public class MapPin : BindableObject
    {
        private string _label;

        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Color bindable property

        public static readonly BindableProperty ColorProperty = BindableProperty.Create<MapPin, MapPinColor>(pin => pin.Color, MapPinColor.Cyan);

        /// <summary>
        /// Color of the pin
        /// </summary>
        public MapPinColor Color
        {
            get
            {
                return (MapPinColor)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        #endregion

        #region Position bindable property

        public static readonly BindableProperty PositionProperty = BindableProperty.Create<MapPin, Position>(pin => pin.Position, new Position(0, 0));

        /// <summary>
        /// Geographical position of the pin
        /// </summary>
        public Position Position
        {
            get { return (Position)GetValue(PositionProperty); }
            set
            {
                SetValue(PositionProperty, value);
            }
        }

        #endregion

        public string Id { get; internal set; }

        public event EventHandler Clicked;

        /// <summary>
        /// Fire Clicked event
        /// </summary>
        protected internal virtual void OnClicked()
        {
            if (Clicked != null)
                Clicked(this, EventArgs.Empty);
        }
    }
}
