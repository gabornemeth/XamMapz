//
// MapPin.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2023, Gabor Nemeth
//

using Microsoft.Maui.Controls.Maps;
using XamMapz.Handlers;

namespace XamMapz
{
    /// <summary>
    /// Map pin
    /// </summary>
    public class PinX : Pin
    {
        #region Color bindable property

		public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(PinColor), typeof(PinX), PinColor.Default);

        /// <summary>
        /// Color of the pin
        /// </summary>
        public PinColor Color
        {
            get
            {
                return (PinColor)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        #endregion

        private readonly MapX _map;

        public MapXHandler MapHandler => _map.Handler as MapXHandler;

        public PinX(MapX map)
        {
            _map = map;
        }
    }
}
