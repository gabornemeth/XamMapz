using System;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XamMapz.Extensions;

namespace XamMapz.Sample
{
    public class TestPage : ContentPage
    {
        private readonly Map _map;

        public TestPage()
        {
            _map = new Map();
            _map.ViewChanged += map_ViewChanged;
            this.Padding = new Thickness(5);
            var grid = new Grid { RowDefinitions = new RowDefinitionCollection() };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.Children.Add(_map);
            Grid.SetRow(_map, 0);
            var buttons = new StackLayout { Orientation = StackOrientation.Horizontal };
            var buttonZOrder = new Button { Text = "Z order" };
            buttonZOrder.Clicked += ButtonZOrder_Clicked;
            buttons.Children.Add(buttonZOrder);
            grid.Children.Add(buttons);
            Grid.SetRow(buttons, 1);

            Content = grid;
        }

        private void AddPolyline(Position center, Color color, float zIndex, double stepLatitude = 0.0, double stepLongitude = 0.0)
        {
            var step = 0.01;
            bool up = true;
            var start = center.Offset(0, -15 * step);

            var polyline = new MapPolyline()
            {
                Color = color,
                Width = 5,
                ZIndex = zIndex,
            };

            for (var i = 0; i < 30; i++)
            {
                polyline.Positions.Add(start.Offset((step + stepLatitude) * (up ? 1 : -1), i * (step + stepLongitude)));
                up = !up;
            }

            var last = polyline.Positions.Last();
            _map.Polylines.Add(polyline);
            var pin = new MapPin { Position = last, Label = last.ToString(), Color = MapPinColor.Red };
            pin.Clicked += delegate
            {
                pin.Label = "Clicked!";
                if (pin.Color == MapPinColor.Red)
                {
                    pin.Color = MapPinColor.Green;
                    polyline.Color = Color.Green;
                }
                else
                {
                    pin.Color = MapPinColor.Red;
                    polyline.Color = Color.Orange;
                }
            };
            _map.Pins.Add(pin);
        }

        private void ButtonZOrder_Clicked(object sender, EventArgs e)
        {
            var polylines = _map.Polylines.ToArray();

            if (polylines.Length < 2) return;

            var tmp = polylines[0].ZIndex;
            polylines[0].ZIndex = polylines[1].ZIndex;
            polylines[1].ZIndex = tmp;
        }

        private void map_ViewChanged(object sender, MapViewChangeEventArgs e)
        {
            Debug.WriteLine($"Map view changed: Lat={e.Span.Center.Latitude} Lon={e.Span.Center.Longitude} r={e.Span.Radius.Kilometers} metres");
        }

        bool _shapesAdded = false;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_shapesAdded) return;

            var center = new Position(46.83, 16.83);

            _map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(5)));
            _map.Pins.Add(new MapPin { Label = "Center", Position = center, Color = MapPinColor.Green });
            _map.Pins.Add(new MapPin { Label = "Offset1", Position = center.Offset(0.01, 0.0), Color = MapPinColor.Blue });
            _map.Pins.Add(new MapPin { Label = "Offset2", Position = center.Offset(-0.01, 0.01) });

            AddPolyline(center, Color.Aqua, zIndex: 2, stepLatitude: 0.003);
            AddPolyline(center, Color.Orange, zIndex: 1, stepLatitude: 0.01, stepLongitude: 0.002);
            _shapesAdded = true;
        }
    }
}