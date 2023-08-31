using Android.Gms.Maps.Model;
using XamMapz.Droid;

namespace XamMapz.Handlers
{
    public partial class MapPinHandler
    {
        private MapPin View => VirtualView as MapPin;

        public override void UpdateValue(string property)
        {
            if (property == MapPin.ColorProperty.PropertyName)
            {
                PlatformView.SetIcon(BitmapDescriptorFactory.DefaultMarker(View.Color.ToAndroidMarkerHue()));
            }
            else base.UpdateValue(property);
        }
    }
}
