using Microsoft.Maui.Maps;
using System.Diagnostics;

namespace XamMapz.Sample
{
    public class TestPage : ContentPage
    {
        private readonly MapX _map;

        public TestPage()
        {
            _map = new MapX();
            _map.ViewChanged += map_ViewChanged;
            this.Padding = new Thickness(5);
            var grid = new Grid { RowDefinitions = new RowDefinitionCollection(new[] { new RowDefinition(GridLength.Star), new RowDefinition(GridLength.Auto) }) };
            grid.Children.Add(_map);
            Grid.SetRow(_map, 0);
            var buttons = new StackLayout { Orientation = StackOrientation.Horizontal };
            var buttonZOrder = new Button { Text = "Z order" };
            buttonZOrder.Clicked += ButtonZOrder_Clicked;
            buttons.Add(buttonZOrder);
            grid.Children.Add(buttons);
            Grid.SetRow(buttons, 1);

            Content = grid;
        }

        private void AddPolyline(Location center, Color color, float zIndex, double stepLatitude = 0.0, double stepLongitude = 0.0)
        {
            var step = 0.01;
            bool up = true;
            var start = center.Offset(0, -15 * step);

            var polyline = new PolylineX
            {
                StrokeColor = color,
                StrokeWidth = 5,
                ZIndex = zIndex,
            };

            for (var i = 0; i < 30; i++)
            {
                polyline.Add(start.Offset((step + stepLatitude) * (up ? 1 : -1), i * (step + stepLongitude)));
                up = !up;
            }

            _map.MapElements.Add(polyline);
        }

        private void ButtonZOrder_Clicked(object sender, EventArgs e)
        {
            var polylines = _map.MapElements
                .OfType<PolylineX>()
                .ToArray();

            if (polylines.Length < 2) return;

            _map.MapElements.Remove(polylines[1]);

            var tmp = polylines[0].ZIndex;
            polylines[0].ZIndex = polylines[1].ZIndex;
            polylines[1].ZIndex = tmp;

            _map.MapElements.Add(polylines[1]);
        }

        private void map_ViewChanged(object sender, MapViewChangeEventArgs e)
        {
            Debug.WriteLine($"Map view changed: {e.Span.Center} r={e.Span.Radius.Kilometers} metres");
        }

        bool _shapesAdded = false;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_shapesAdded) return;

            var center = new Location(46.83, 16.83);

            _map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(5)));
            _map.Pins.Add(new PinX { Label = "Center", Location = center, Color = PinColor.Green });
            _map.Pins.Add(new PinX { Label = "Offset1", Location = center.Offset(0.01, 0.0), Color = PinColor.Blue });
            _map.Pins.Add(new PinX { Label = "Offset2", Location = center.Offset(-0.01, 0.01) });

            AddPolyline(center, Colors.Aqua, zIndex: 2, stepLatitude: 0.003);
            AddPolyline(center, Colors.Orange, zIndex: 1, stepLatitude: 0.01, stepLongitude: 0.002);
            _shapesAdded = true;
        }
    }
}
