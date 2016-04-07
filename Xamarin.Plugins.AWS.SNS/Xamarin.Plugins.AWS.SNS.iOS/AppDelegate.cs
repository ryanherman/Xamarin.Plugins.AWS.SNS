using System;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Plugins.AWS.SNS.Helpers;

namespace Xamarin.Plugins.AWS.SNS.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Forms.Init();
            LoadApplication(new App());
            /* Ask to Authorize Push Messaging */
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Sound |
                                                                              UIUserNotificationType.Alert |
                                                                              UIUserNotificationType.Badge, null);

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Badge |
                                                                                   UIRemoteNotificationType.Sound |
                                                                                   UIRemoteNotificationType.Alert);
            }
            /* Ask to Authorize Push Messaging */
            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData token)
        {
            var deviceToken = token.Description.Replace("<", "").Replace(">", "").Replace(" ", "");
            if (!string.IsNullOrEmpty(deviceToken))
            {
                SNSUtils.RegisterDevice(SNSUtils.Platform.IOS, deviceToken);
            }
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo,
            Action<UIBackgroundFetchResult> completionHandler)
        {
            var payload = DictionaryToJson(userInfo);
            SNSUtils.HandleMessage(payload);
        }

        private static string DictionaryToJson(NSDictionary dictionary)
        {
            NSError error;
            var json = NSJsonSerialization.Serialize(dictionary, NSJsonWritingOptions.PrettyPrinted, out error);

            return json.ToString(NSStringEncoding.UTF8);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            Console.WriteLine(@"Failed to register for remote notification {0}", error.Description);
        }
    }
}