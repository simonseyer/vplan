using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Diagnostics;
using Android.Support.V4.App;
using Android.Media;
using Android.App;
using System.Linq;

namespace FLSVertretungsplan.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class AlarmReceiver : BroadcastReceiver
    {
        static string WakeLockTag = "vplan_background_refresh";
        static string ChannelId = "vplan_channel_01";
        static int NotificationId = 1;

        public override void OnReceive(Context context, Intent intent)
        {
            var powerManager = PowerManager.FromContext(context);
            var wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, WakeLockTag);
            wakeLock.Acquire();

            var dataStore = ServiceLocator.Instance.Get<IVplanDataStore>();
            Task.Run(async () =>
            {
                try {
                    var diff = await dataStore.Refresh();

                    if (diff.NewBookmarkedChanges.Any() || diff.NewNewClasses.Any())
                    {
                        string text;
                        if (diff.NewBookmarkedChanges.Any())
                        {
                            text = diff.NewBookmarkedChanges.Count() + " neue Einträge";
                        }
                        else
                        {
                            text = diff.NewNewClasses.Count() + " neue Klassen";
                        }

                        var notificationManager = NotificationManager.FromContext(context);

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                        {
                            var channel = new NotificationChannel(ChannelId, "Vertretungsplan", NotificationImportance.High);
                            channel.EnableLights(true);
                            channel.EnableVibration(true);
                            notificationManager.CreateNotificationChannel(channel);
                        }

                        var resultIntent = new Intent(context, typeof(MainActivity));
                        var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(context);
                        stackBuilder.AddNextIntentWithParentStack(resultIntent);
                        var pendingIntent = stackBuilder.GetPendingIntent(0, 0);

                        var builder = new NotificationCompat.Builder(context, ChannelId)
                                                            .SetContentTitle("Neuer Vertretungsplan verfügbar")
                                                            .SetContentText(text)
                                                            .SetSmallIcon(Resource.Mipmap.Icon)
                                                            .SetPriority(NotificationCompat.PriorityMax)
                                                            .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
                                                            .SetDefaults(NotificationCompat.DefaultVibrate | NotificationCompat.DefaultLights)
                                                            .SetContentIntent(pendingIntent)
                                                            .SetAutoCancel(true);
                        notificationManager.Notify(NotificationId, builder.Build());
                    }
                }
                catch (Exception e)
                {
                    
                }
                finally
                {
                    wakeLock.Release();
                }
            });
        }
    }
}
