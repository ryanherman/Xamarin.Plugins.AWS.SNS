The beginning of my first nuget package!

### Installation

Xamarin.SNS.Plugin!
===================
Xamarin.SNS.Plugin is an easy to use NuGet package to get AWS SNS up and running quickly!

Install
-------------

Android:
=============
You will need to modify your Manifest file to include certain permissions, here is a sample.  Make sure you change all the <app.namespace> to yours.

       `<?xml version="1.0" encoding="utf-8"?>
    	<manifest xmlns:android="http://schemas.android.com/apk/res/android" package="<app.namespace>" android:installLocation="auto" android:versionName="" android:versionCode="1">
    	<uses-sdk android:targetSdkVersion="23" android:minSdkVersion="16" />
    	<uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
    	<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
    	<uses-permission android:name="android.permission.INTERNET" />
    	<uses-permission android:name="android.permission.MEDIA_CONTENT_CONTROL" />
    	<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
    	<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
    	<uses-permission android:name="android.permission.CAMERA" />
    	<uses-permission android:name="android.permission.WAKE_LOCK" />
    	<uses-permission android:name="com.google.android.c2dm.permission.RECEIVE" />
    	<uses-permission android:name="com.google.android.c2dm.permission.REGISTER" />
    	<uses-permission android:name="android.permission.RECEIVE_BOOT_COMPLETED" />
    	<permission android:name="<app.namespace>.permission.C2D_MESSAGE" android:protectionLevel="signature" />
    	<uses-permission android:name="<app.namespace>.permission.C2D_MESSAGE" />
    	<!-- Receiver GCM -->
    	<receiver android:name="<app.namespace>.GcmBroadcastReceiver" android:permission="com.google.android.c2dm.permission.SEND">
    		<intent-filter>
    			<action android:name="com.google.android.c2dm.intent.RECEIVE" />
    			<category android:name="<app.namespace>" />
    		</intent-filter>
    	</receiver>
    	<!-- Service GCM -->
    	<service android:exported="true" android:name="<app.namespace>.GcmIntentService" />
    </manifest>`

IOS:
=============
You will need to ask the user to authorize push messaging so in your FinishLaunching in your AppDelegate.cs add:

            `if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var settings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Sound |
                                           UIUserNotificationType.Alert | UIUserNotificationType.Badge, null);

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Badge |
                                                                         UIRemoteNotificationType.Sound | UIRemoteNotificationType.Alert);
            }`

Then you will need to handle the push messaging on messages coming in, registration, and errors:

	`public override void RegisteredForRemoteNotifications(UIApplication application, NSData token)
        {
            var deviceToken = token.Description.Replace("<", "").Replace(">", "").Replace(" ", "");
            if (!string.IsNullOrEmpty(deviceToken))
            {
                SNSUtils.RegisterDevice(SNSUtils.Platform.IOS, deviceToken);
            }
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
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
        }`
 
License
----

MIT