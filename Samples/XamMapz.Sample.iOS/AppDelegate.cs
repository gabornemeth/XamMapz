using Foundation;
using UIKit;

namespace XamMapz.Sample.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            Xamarin.Forms.Forms.Init();
            Xamarin.FormsMaps.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(application, launchOptions);
        }
    }
}


