//
// MapRoute.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2015, Gabor Nemeth
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Linq;

namespace XamMapz
{
    /// <summary>
    /// Polyline
    /// </summary>
    public class MapPolyline : BindableObject
    {
        private ObservableCollection<Position> _positions;
        private EventSuspender _suspendPositionsChanged = new EventSuspender();

        public const string PositionsProperty = "Positions";

        public IList<Position> Positions
        {
            get
            {
                return _positions;
            }
            set
            {
                _suspendPositionsChanged.Suspend();
                try
                {
                    foreach (var pos in value)
                    {
                        _positions.Add(pos);
                    }
                    OnPropertyChanged();
                }
                finally
                {
                    _suspendPositionsChanged.Allow();
                }
            }
        }


        #region Color bindable property

        public static readonly BindableProperty ColorProperty = BindableProperty.Create<MapPolyline, Color>(route => route.Color, Color.Default);

        public Color Color
        {
            get
            {
                return (Color)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        #endregion

        public double Width { get; set; }

        #region ZIndex bindable property

        public static readonly BindableProperty ZIndexProperty = BindableProperty.Create<MapPolyline, float>(route => route.ZIndex, 1.0f);

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
            _positions = new ObservableCollection<Position>();
            _positions.CollectionChanged += Positions_CollectionChanged;
        }

        private void Positions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_suspendPositionsChanged.IsSuspended)
                return;
            
            if (PositionChanged != null)
                PositionChanged(this, e);
        }
    }
}
