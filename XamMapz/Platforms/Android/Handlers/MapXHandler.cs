using Android.Gms.Maps.Model;
using XamMapz.Droid;

namespace XamMapz.Handlers
{
    partial class MapXHandler : Microsoft.Maui.Maps.Handlers.MapHandler, IMapXHandler
    {
        public Point? ProjectToScreen(Location location)
        {
            var screenPos = Map?.Projection.ToScreenLocation(location.ToLatLng());
            if (screenPos == null) return null;

            return new Point(screenPos.X, screenPos.Y);
        }

        public Marker GetNativeMarker(object? id)
        {
            var markersField = typeof(Microsoft.Maui.Maps.Handlers.MapHandler).GetField("_markers", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var markers = markersField.GetValue(this) as List<Marker>;
            if (markers == null) return null;

            return markers.FirstOrDefault(m => m.Id.Equals(id));
        }

        public Polyline GetNativePolyline(object? id)
        {
            var polylinesField = typeof(Microsoft.Maui.Maps.Handlers.MapHandler).GetField("_polylines", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var polylines = polylinesField.GetValue(this) as List<Polyline>;
            if (polylines == null) return null;

            return polylines.FirstOrDefault(p => p.Id.Equals(id));
        }
    }
}
