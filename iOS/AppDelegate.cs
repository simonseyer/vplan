using Foundation;
using UIKit;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UserNotifications;
using System.Linq;

namespace FLSVertretungsplan.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate, INotificationActivationDelegate
    {
        // class-level declarations

        const int REFRESH_ON_APP_START_DELAY = 600;

        public override UIWindow Window
        {
            get;
            set;
        }

        bool InitialRun = true;

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            AppCenter.Start("***REMOVED***", typeof(Analytics), typeof(Crashes));

            App.Initialize();

            var navigationController = Window.RootViewController as UINavigationController;
            var storyboard = UIStoryboard.FromName("Main", NSBundle.MainBundle);

            ISettingsDataStore settingsDataSore = ServiceLocator.Instance.Get<ISettingsDataStore>();

            if (settingsDataSore.FirstAppLaunch)
            {
                settingsDataSore.FirstAppLaunch = false;
                var setupViewController = storyboard.InstantiateViewController("SetupViewController") as SetupViewController;
                setupViewController.NotificationActivationDelegate = this;
                navigationController.ViewControllers = new UIViewController[]
                {
                    setupViewController
                };
            }
            else
            {
                _ = RequestNotifications();
                navigationController.ViewControllers = new UIViewController[]
                {
                    storyboard.InstantiateViewController("MainViewController")
                };
            }

            var dataStore = ServiceLocator.Instance.Get<IVplanDataStore>();
            Task.Run(() =>
            {
                dataStore.Load();
            });

            return true;
        }

        public override void OnActivated(UIApplication application)
        {
            if (!InitialRun)
            {
                return;
            }
            InitialRun = false;

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            _ = Refresh();
        }

        async Task Refresh()
        {
            var dataStore = ServiceLocator.Instance.Get<IVplanDataStore>();
            await Task.Delay(REFRESH_ON_APP_START_DELAY);
            await dataStore.Refresh();
        }

        public void ActivateNotifications()
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

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            var fetch = FetchData();
            fetch.ContinueWith(r =>
            {
                if (r.IsFaulted)
                {
                    completionHandler(UIBackgroundFetchResult.Failed);
                }
                else
                {
                    completionHandler(r.Result ? UIBackgroundFetchResult.NewData : UIBackgroundFetchResult.NoData);
                }
            });
        }

        async Task<bool> FetchData()
        {
            var dataStore = ServiceLocator.Instance.Get<IVplanDataStore>();
            await dataStore.Load();
            var diff = await dataStore.Refresh();
            if (diff == null)
            {
                throw new Exception();
            }

            var notificationCenter = UNUserNotificationCenter.Current;
            var settings = await notificationCenter.GetNotificationSettingsAsync();
            var oldNotifications = await notificationCenter.GetPendingNotificationRequestsAsync();

            if (settings.AuthorizationStatus == UNAuthorizationStatus.Authorized &&
                oldNotifications.Length == 0 &&
                (diff.NewBookmarkedChanges.Any() || diff.NewNewSchoolClassBookmarks.Any()))
            {
                var content = new UNMutableNotificationContent
                {
                    Title = NSBundle.MainBundle.LocalizedString("new_plan_title", "")
                };
                if (diff.NewBookmarkedChanges.Any())
                {
                    if (diff.NewBookmarkedChanges.Count() == 1)
                    {
                        content.Body = NSBundle.MainBundle.LocalizedString("new_changes_singular", "");
                    }
                    else
                    {
                        var text = NSBundle.MainBundle.LocalizedString("new_changes_plural", "");
                        content.Body = NSString.LocalizedFormat(text, diff.NewBookmarkedChanges.Count());
                    }
                    content.Badge = diff.NewBookmarkedChanges.Count();
                }
                else
                {
                    if (diff.NewNewSchoolClassBookmarks.Count() == 1)
                    {
                        content.Body = NSBundle.MainBundle.LocalizedString("new_school_classes_singular", "");
                    }
                    else
                    {
                        var text = NSBundle.MainBundle.LocalizedString("new_school_classes_plural", "");
                        content.Body = NSString.LocalizedFormat(text, diff.NewNewSchoolClassBookmarks.Count());
                    }
                    content.Badge = diff.NewNewSchoolClassBookmarks.Count();
                }

                var request = UNNotificationRequest.FromIdentifier(dataStore.Vplan.Value.LastUpdate.ToString(), content, null);
                await notificationCenter.AddNotificationRequestAsync(request);
            }

            return diff.Updated;
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

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}
