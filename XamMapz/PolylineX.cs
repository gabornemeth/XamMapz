//
// PolylineX.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2023, Gabor Nemeth
//

using Microsoft.Maui.Controls.Maps;
using System.Collections.Specialized;

namespace XamMapz
{
    /// <summary>
    /// Polyline
    /// </summary>
    public class PolylineX : Polyline
    {
        #region ZIndex bindable property

        public static readonly BindableProperty ZIndexProperty = BindableProperty.Create(nameof(ZIndex), typeof(float), typeof(PolylineX), 1.0f);

        public float ZIndex
        {
            get => (float)GetValue(ZIndexProperty);
            set => SetValue(ZIndexProperty, value);
        }

        #endregion

        public event NotifyCollectionChangedEventHandler PositionChanged;

        public override string ToString()
        {
            return string.Format("<MapPolyline: 0x{0:x}>", GetHashCode());
        }
    }
}
