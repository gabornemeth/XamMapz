//
// MapPolyline.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace XamMapz
{
    /// <summary>
    /// Polyline
    /// </summary>
    public class MapPolyline : Polyline
    {
        #region ZIndex bindable property

		public static readonly BindableProperty ZIndexProperty = BindableProperty.Create(nameof(ZIndex), typeof(float), typeof(MapPolyline), 1.0f);

        public float ZIndex
        {
            get
            {
                return (float)GetValue(ZIndexProperty);
            }
            set
            {
                SetValue(ZIndexProperty, value);
            }
        }

        #endregion

        public event NotifyCollectionChangedEventHandler PositionChanged;

        public MapPolyline()
        {
            ZIndex = 1;
        }

        public override string ToString()
        {
            return string.Format("<MapPolyline: 0x{0:x}>", GetHashCode());
        }
    }
}
