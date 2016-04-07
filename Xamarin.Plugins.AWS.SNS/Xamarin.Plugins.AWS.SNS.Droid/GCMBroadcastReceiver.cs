using Android.App;
using Android.Content;

namespace Xamarin.Plugins.AWS.SNS.Droid
{
    [BroadcastReceiver(Permission = "com.google.android.c2dm.permission.SEND", Enabled = true)]
    [IntentFilter(new[] {"com.google.android.c2dm.intent.RECEIVE"}, Categories = new[] {"com.oneiota.consumerapp"})]
    [IntentFilter(new[] {"com.google.android.c2dm.intent.REGISTRATION"}, Categories = new[] {"com.oneiota.consumerapp"})
    ]
    [IntentFilter(new[] {"com.google.android.gcm.intent.RETRY"}, Categories = new[] {"com.oneiota.consumerapp"})]
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
    [IntentFilter(new[] {Intent.ActionBootCompleted})]
    public class GCMBootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            GCMIntentService.RunIntentInService(context, intent);
            SetResult(Result.Ok, null, null);
        }
    }
}