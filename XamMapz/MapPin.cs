//
// MapPin.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2023, Gabor Nemeth
//

using Microsoft.Maui.Controls.Maps;

namespace XamMapz
{
    /// <summary>
    /// Map pin
    /// </summary>
    public class MapPin : Pin
    {
        #region Color bindable property

		public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(MapPinColor), typeof(MapPin), MapPinColor.Default);

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
    }
}
