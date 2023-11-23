using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using XamMapz.Extensions;

namespace XamMapz.Sample
{
    public class TestPage : TabbedPage
    {
        private static readonly Position Center = new Position(46.83, 16.83);
        private Map _map;
        private Grid _container;
        private MapPin _tempPin;

        public TestPage()
        {
            var mapPage = new ContentPage { Title = "map" };

            _map = new Map();
            _map.MoveToRegion(MapSpan.FromCenterAndRadius(Center, Distance.FromKilometers(5)));
            _map.ViewChanged += map_ViewChanged;
            this.Padding = new Thickness(5);
            var grid = new Grid { RowDefinitions = new RowDefinitionCollection() };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.Children.Add(_map);
            Grid.SetRow(_map, 0);

            _container = grid;

            var buttons = new StackLayout { Orientation = StackOrientation.Horizontal };
            var buttonZOrder = new Button { Text = "Z order" };
            buttonZOrder.Clicked += ButtonZOrder_Clicked;
            var buttonReadd = new Button { Text = "Re-add map" };
            buttonReadd.Clicked += ButtonReadd_Clicked;
            var buttonAdd = new Button { Text = "Add" };
            buttonAdd.Clicked += ButtonAdd_Clicked;
            var buttonRemove = new Button { Text = "Remove" };
            buttonRemove.Clicked += ButtonRemove_Clicked;

            buttons.Children.Add(buttonZOrder);
            buttons.Children.Add(buttonReadd);
            buttons.Children.Add(buttonAdd);
            buttons.Children.Add(buttonRemove);

            grid.Children.Add(buttons);
            Grid.SetRow(buttons, 1);

            mapPage.Content = grid;

            Children.Add(mapPage);

            var otherPage = new ContentPage { Title = "other" };
            otherPage.Content = new Button { Text = "Click me!" };
            Children.Add(otherPage);

            _tempPin = new MapPin
            {
                Label = "Temporary #1",
                Color = MapPinColor.Violet,
                Position = Center.Offset(-0.02, 0.02)
            };

            SetupMap();
            _map.IsVisible = false;
        }

        private void ButtonZOrder_Clicked(object sender, EventArgs e)
        {
            var polylines = _map.Polylines.ToArray();

            if (polylines.Length < 2) return;

            var tmp = polylines[0].ZIndex;
            polylines[0].ZIndex = polylines[1].ZIndex;
            polylines[1].ZIndex = tmp;
        }

        private void ButtonRemove_Clicked(object sender, EventArgs e)
        {
            if (_map.Pins.Contains(_tempPin))
            {
                _map.Pins.Remove(_tempPin);
            }
        }

        private void ButtonAdd_Clicked(object sender, EventArgs e)
        {
            if (_map.Pins.Contains(_tempPin) == false)
            {
                _map.Pins.Add(_tempPin);
            }
        }

        private void ButtonReadd_Clicked(object sender, EventArgs e)
        {
            // remove old map
            _map.ViewChanged -= map_ViewChanged;
            _container.Children.Remove(_map);
            
            // add new map
            _map = new Map();
            _map.ViewChanged += map_ViewChanged;
            _container.Children.Add(_map);
            Grid.SetRow(_map, 0);
            SetupMap();
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

        private void map_ViewChanged(object sender, MapViewChangeEventArgs e)
        {
            Debug.WriteLine($"Map view changed: Lat={e.Span.Center.Latitude} Lon={e.Span.Center.Longitude} r={e.Span.Radius.Kilometers} metres");
        }

        private void SetupMap()
        {
            _map.Pins.Add(new MapPin { Label = "Center", Position = Center, Color = MapPinColor.Green });
            _map.Pins.Add(new MapPin { Label = "Offset1", Position = Center.Offset(0.01, 0.0), Color = MapPinColor.Blue });
            _map.Pins.Add(new MapPin { Label = "Offset2", Position = Center.Offset(-0.01, 0.01) });

            AddPolyline(Center, Color.Aqua, zIndex: 2, stepLatitude: 0.003);
            AddPolyline(Center, Color.Orange, zIndex: 1, stepLatitude: 0.01, stepLongitude: 0.002);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(3000); // delay map's appearance
            _map.IsVisible = true;
        }
    }
}
