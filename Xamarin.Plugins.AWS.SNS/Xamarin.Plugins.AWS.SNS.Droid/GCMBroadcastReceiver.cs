using Android.App;
using Android.Content;

namespace Xamarin.Plugins.AWS.SNS.Droid
{
    [BroadcastReceiver(Permission = "com.google.android.c2dm.permission.SEND", Enabled = true)]
    [IntentFilter(new string[] { "com.google.android.c2dm.intent.RECEIVE" }, Categories = new string[] { "com.oneiota.consumerapp" })]
    [IntentFilter(new string[] { "com.google.android.c2dm.intent.REGISTRATION" }, Categories = new string[] { "com.oneiota.consumerapp" })]
    [IntentFilter(new string[] { "com.google.android.gcm.intent.RETRY" }, Categories = new string[] { "com.oneiota.consumerapp" })]

    public class GCMBroadcastReceiver : BroadcastReceiver
    {
        private const string TAG = "PushHandlerBroadcastReceiver";

        public override void OnReceive(Context context, Intent intent)
        {
            GCMIntentService.RunIntentInService(context, intent);
            SetResult(Result.Ok, null, null);
        }
    }

    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]

    public class GCMBootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            GCMIntentService.RunIntentInService(context, intent);
            SetResult(Result.Ok, null, null);
        }
    }
}