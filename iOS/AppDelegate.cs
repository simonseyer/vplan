using Foundation;
using UIKit;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UserNotifications;

namespace FLSVertretungsplan.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            AppCenter.Start("***REMOVED***", typeof(Analytics), typeof(Crashes));

            App.Initialize();

            RequestNotifications();
            var dataStore = ServiceLocator.Instance.Get<IVplanDataStore>();
            var _ = dataStore.GetVplanAsync(true);

            return true;
        }

        private async void RequestNotifications()
        {
            var notificationCenter = UNUserNotificationCenter.Current;
            var result = await notificationCenter.RequestAuthorizationAsync(UNAuthorizationOptions.Alert);
            if (!result.Item1)
            {
                Debug.WriteLine("User denied notifications: " + result.Item2.LocalizedDescription);
            }
        }

        public override void PerformFetch(UIApplication application, System.Action<UIBackgroundFetchResult> completionHandler)
        {
            try
            {
                var fetch = FetchData();
                fetch.Wait();
                completionHandler(fetch.Result ? UIBackgroundFetchResult.NewData : UIBackgroundFetchResult.NoData);
            }
            catch (Exception)
            {
                completionHandler(UIBackgroundFetchResult.Failed);
            }
        }

        private async Task<bool> FetchData()
        {
            var dataStore = ServiceLocator.Instance.Get<IVplanDataStore>();

            var oldVplan = await dataStore.GetVplanAsync();
            var newVplan = await dataStore.GetVplanAsync(true);

            var gotUpdated = newVplan != null && (oldVplan == null || !oldVplan.LastUpdate.Equals(newVplan.LastUpdate));

            var notificationCenter = UNUserNotificationCenter.Current;
            var settings = await notificationCenter.GetNotificationSettingsAsync();
            var oldNotifications = await notificationCenter.GetPendingNotificationRequestsAsync();
            if (settings.AuthorizationStatus == UNAuthorizationStatus.Authorized && oldNotifications.Length == 0)
            {
                var content = new UNMutableNotificationContent();
                content.Title = "Neuer Vertretungsplan verfügbar";
                content.Body = newVplan.Changes.Count + " Einträge";
                var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0, false);
                var request = UNNotificationRequest.FromIdentifier("", content, trigger);
                await notificationCenter.AddNotificationRequestAsync(request);
            }

            return gotUpdated;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}
