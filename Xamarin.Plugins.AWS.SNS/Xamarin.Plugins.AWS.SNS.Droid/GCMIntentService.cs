using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Newtonsoft.Json.Linq;
using Xamarin.Plugins.AWS.SNS.Helpers;
using Debug = System.Diagnostics.Debug;

#pragma warning disable 4014

namespace Xamarin.Plugins.AWS.SNS.Droid
{
    [Service]
    public class GCMIntentService : IntentService
    {
        private static PowerManager.WakeLock sWakeLock;
        private static readonly object LOCK = new object();

        public static void RunIntentInService(Context context, Intent intent)
        {
            lock (LOCK)
            {
                if (sWakeLock == null)
                {
                    // This is called from BroadcastReceiver, there is no init.
                    var pm = PowerManager.FromContext(context);
                    sWakeLock = pm.NewWakeLock(
                        WakeLockFlags.Partial, "My WakeLock Tag");
                }
            }

            sWakeLock.Acquire();
            intent.SetClass(context, typeof (GCMIntentService));
            context.StartService(intent);
        }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                var context = ApplicationContext;
                var action = intent.Action;

                if (action.Equals("com.google.android.c2dm.intent.REGISTRATION"))
                {
                    HandleRegistration(intent);
                }
                else if (action.Equals("com.google.android.c2dm.intent.RECEIVE"))
                {
                    HandleMessage(intent);
                }
            }
            finally
            {
                lock (LOCK)
                {
                    //Sanity check for null as this is a public method
                    if (sWakeLock != null) sWakeLock.Release();
                }
            }
        }

        private void HandleRegistration(Intent intent)
        {
            var registrationId = intent.GetStringExtra("registration_id");
            var error = intent.GetStringExtra("error");
            var unregistration = intent.GetStringExtra("unregistered");

            if (string.IsNullOrEmpty(error))
                SNSUtils.RegisterDevice(SNSUtils.Platform.Android, registrationId);
        }

        private void HandleMessage(Intent intent)
        {
            if (intent.Extras != null && !intent.Extras.IsEmpty)
            {
                Debug.WriteLine("GCM Listener - Push Received");

                var values = new JObject();

                foreach (var key in intent.Extras.KeySet())
                {
                    var value = intent.Extras.Get(key).ToString();
                    values.Add(key, value);

                }
                var message = intent.Extras.GetString("message");

                CreateNotification("Notification", message, intent.Extras);
                SNSUtils.HandleMessage(values.ToString());
            }
        }

        private void CreateNotification(string title, string desc,  Bundle extras)
        {
            // Create notification
            var notificationManager = GetSystemService(NotificationService) as NotificationManager;

            // Create an intent to show UI
            var uiIntent = new Intent(this, typeof(MainActivity));

            if (extras != null) { uiIntent.PutExtras(extras); }
          
            // Create the notification
            var builder = new NotificationCompat.Builder(this);
            
            Notification notification = builder.SetContentIntent(PendingIntent.GetActivity(this, 0, uiIntent, 0))
            .SetSmallIcon(Android.Resource.Drawable.IcMenuAgenda).SetTicker(desc)
            //.SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.iota))
            .SetAutoCancel(true).SetContentTitle(title)
            .SetContentText(desc).Build();

            // Auto cancel will remove the notification once the user touches it
            notification.Flags = NotificationFlags.AutoCancel;

            // Show the notification
            if (notificationManager != null) notificationManager.Notify(1, notification);
        }
    }
}