using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.Platform.Android;
using Xamarin.Plugins.AWS.SNS.Helpers;
using Debug = System.Diagnostics.Debug;

namespace Xamarin.Plugins.AWS.SNS.Droid
{
    [Activity(Label = "Xamarin.Plugins.AWS.SNS", Icon = "@drawable/icon", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Forms.Forms.Init(this, bundle);
            RegisterForGCM();
            LoadApplication(new App());
            if (Intent.Extras != null && !Intent.Extras.IsEmpty)
            {
                Debug.WriteLine("GCM Listener - Push Received");

                var values = new JObject();

                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.Get(key).ToString();
                    values.Add(key, value);
                }
                SNSUtils.HandleMessage(values.ToString());
            }
        }

        private void RegisterForGCM()
        {
            var senders = Constants.GoogleConsoleProjectId;
            var intent = new Intent("com.google.android.c2dm.intent.REGISTER");
            intent.SetPackage("com.google.android.gsf");
            intent.PutExtra("app", PendingIntent.GetBroadcast(this, 0, new Intent(), 0));
            intent.PutExtra("sender", senders);
            StartService(intent);
        }
    }
}