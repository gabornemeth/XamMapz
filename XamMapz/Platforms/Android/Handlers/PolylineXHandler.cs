using Android.Gms.Maps.Model;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps;

namespace XamMapz.Handlers
{
    partial class PolylineXHandler
    {
        static PolylineXHandler()
        {
            Mapper.Add(PolylineX.ZIndexProperty.PropertyName, MapZIndex);
        }

        public PolylineXHandler() : base(Mapper)
        {
        }

        public static void MapZIndex(IMapElementHandler handler, IMapElement polyline)
        {
            if (polyline is PolylineX polylineX)
            {
                var nativePolyline = polylineX.MapHandler.GetNativePolyline(polyline.MapElementId);
                if (handler.PlatformView is PolylineOptions op)
                {
                    if (nativePolyline != null) nativePolyline.ZIndex = polylineX.ZIndex;
                    op.InvokeZIndex(polylineX.ZIndex);
                }
            }
        }
    }
}
