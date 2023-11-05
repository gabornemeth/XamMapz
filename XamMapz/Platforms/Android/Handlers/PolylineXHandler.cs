using Android.Gms.Maps.Model;
using Microsoft.Maui.Maps.Handlers;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

namespace XamMapz.Handlers
{
    partial class PolylineXHandler
    {
        static PolylineXHandler()
        {
            Mapper.Add(PolylineX.ZIndexProperty.PropertyName, MapZIndex);
            Mapper.AppendToMapping(PolylineX.StrokeColorProperty.PropertyName, MapStrokeColor);
            Mapper.AppendToMapping(PolylineX.StrokeWidthProperty.PropertyName, MapStrokeWidth);
        }

        public PolylineXHandler() : base(Mapper)
        {
        }

        public static void MapZIndex(IMapElementHandler handler, IMapElement element)
        {
            if (element is PolylineX polyline)
            {
                var nativePolyline = polyline.MapHandler.GetNativePolyline(polyline.MapElementId);
                if (handler.PlatformView is PolylineOptions op)
                {
                    if (nativePolyline != null) nativePolyline.ZIndex = polyline.ZIndex;
                    op.InvokeZIndex(polyline.ZIndex);
                }
            }
        }

        public static void MapStrokeColor(IMapElementHandler handler, IMapElement element)
        {
            if (element is PolylineX polyline)
            {
                var nativePolyline = polyline.MapHandler.GetNativePolyline(polyline.MapElementId);
                if (nativePolyline != null) nativePolyline.Color = polyline.StrokeColor.ToAndroid();
                if (handler.PlatformView is PolylineOptions op)
                {
                    op.InvokeColor(polyline.StrokeColor.ToAndroid());
                }
            }
        }

        public static void MapStrokeWidth(IMapElementHandler handler, IMapElement element)
        {
            if (element is PolylineX polyline)
            {
                var nativePolyline = polyline.MapHandler.GetNativePolyline(polyline.MapElementId);
                if (nativePolyline != null) nativePolyline.Width = polyline.StrokeWidth;
                if (handler.PlatformView is PolylineOptions op)
                {
                    op.InvokeWidth(polyline.StrokeWidth);
                }
            }

        }
    }
}
