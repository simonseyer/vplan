using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UserNotifications;

namespace FLSVertretungsplan.iOS
{
    public class NotificationHandler
    {
        public  void ActivateNotifications()
        {
            _ = RequestNotifications();
        }

        async Task RequestNotifications()
        {
            var notificationCenter = UNUserNotificationCenter.Current;
            var result = await notificationCenter.RequestAuthorizationAsync(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge);
            if (!result.Item1)
            {
                Debug.WriteLine("User denied notifications: " + result.Item2.LocalizedDescription);
            }
        }
    }
}
