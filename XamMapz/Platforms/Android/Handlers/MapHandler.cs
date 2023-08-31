using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamMapz.Messaging;

namespace XamMapz.Platforms.Android.Handlers
{
    partial class MapHandler
    {
        //MessagingCenter.Subscribe<XamMapz.Map, MapMessage>(this, MapMessage.Message, (sender, message) =>
        //            {
        //                // Handle only messages sent by Element
        //                if (sender != MapEx)
        //                    return;

        //OnMapMessage(sender, message);
        //            });

        //protected virtual void OnMapMessage(Map map, MapMessage message)
        //{
        //    if (message is ZoomMessage)
        //    {
        //        //var msg = (ZoomMessage)message;
        //        UpdateRegion();
        //    }
        //    else if (message is ProjectionMessage)
        //    {
        //        var msg = (ProjectionMessage)message;
        //        var screenPos = NativeMap.Projection.ToScreenLocation(msg.Location.ToLatLng());
        //        msg.ScreenPosition = new Point(screenPos.X, screenPos.Y);
        //    }
        //    else if (message is MoveMessage moveMessage)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"MoveMessage received: {moveMessage.Target.Latitude} {moveMessage.Target.Longitude}");
        //        UpdateGoogleMap((formsMap, nativeMap) =>
        //        {
        //            var cameraUpdate = CameraUpdateFactory.NewLatLng(moveMessage.Target.ToLatLng());
        //            nativeMap.MoveCamera(cameraUpdate);
        //        });

        //    }
        //}

    }
}
