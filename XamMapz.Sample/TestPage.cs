using Microsoft.Maui.Maps;

namespace XamMapz.Sample
{
    public class TestPage : ContentPage
    {
        XamMapz.Map _map;

        public TestPage()
        {
            _map = new XamMapz.Map();
            this.Padding = new Thickness(5);
            Content = _map;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var center = new Location(46.83, 16.83);

            _map.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(5)));
            _map.Pins.Add(new MapPin { Label = "Center", Location = center, Color = MapPinColor.Green });
            _map.Pins.Add(new MapPin { Label = "Offset1", Location = center.Offset(0.01, 0.0), Color = MapPinColor.Blue });
            _map.Pins.Add(new MapPin { Label = "Offset2", Location = center.Offset(-0.01, 0.01) });
        }
    }
}
