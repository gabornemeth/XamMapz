using Android.Gms.Maps.Model;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps;
using XamMapz.Droid;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Controls.Maps;

namespace XamMapz.Handlers
{
    public partial class MapPinHandler
    {
        static MapPinHandler()
        {
            Mapper.Add(MapPin.ColorProperty.PropertyName, MapPinColor);
        }

        public MapPinHandler() : base(Mapper)
        {
        }

        public static void MapPinColor(IMapPinHandler handler, IMapPin pin)
        {
            if (pin is MapPin mapPin)
            {
                if (mapPin.Color == XamMapz.MapPinColor.Default)
                {
                    handler.PlatformView.SetIcon(null);
                }
                else
                {
                    handler.PlatformView.SetIcon(BitmapDescriptorFactory.DefaultMarker(mapPin.Color.ToAndroidMarkerHue()));
                }
            }
        }
    }
}
