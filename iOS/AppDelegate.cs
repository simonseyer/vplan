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
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        const int REFRESH_ON_APP_START_DELAY = 600;
        const string APP_CENTER_IDENTIFIER = "**REMOVED**";

        public override UIWindow Window { get; set; }

        bool InitialActivation = true;

        BackgroundFetchHandler BackgroundFetchHandler => new BackgroundFetchHandler();
        NotificationHandler NotificationHandler => new NotificationHandler();
        ISettingsDataStore SettingsDataSore => ServiceLocator.Instance.Get<ISettingsDataStore>();

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            AppCenter.Start(APP_CENTER_IDENTIFIER, typeof(Analytics), typeof(Crashes));
            BackgroundFetchHandler.Setup();
            App.Initialize();

            var navigationController = Window.RootViewController as UINavigationController;
            var storyboard = UIStoryboard.FromName("Main", NSBundle.MainBundle);

            if (SettingsDataSore.FirstAppLaunch)
            {
                SettingsDataSore.FirstAppLaunch = false;
                var setupViewController = storyboard.InstantiateViewController("SetupViewController") as SetupViewController;
                setupViewController.NotificationHandler = NotificationHandler;
                navigationController.ViewControllers = new UIViewController[]
                {
                    setupViewController
                };
            }
            else
            {
                NotificationHandler.ActivateNotifications();
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
            if (!InitialActivation)
            {
                return;
            }
            InitialActivation = false;

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            _ = Refresh();
        }

        async Task Refresh()
        {
            var dataStore = ServiceLocator.Instance.Get<IVplanDataStore>();
            await Task.Delay(REFRESH_ON_APP_START_DELAY);
            await dataStore.Refresh();
        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            BackgroundFetchHandler.Fetch().ContinueWith(r =>
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

        public override void OnResignActivation(UIApplication application) { }

        public override void DidEnterBackground(UIApplication application) { }

        public override void WillEnterForeground(UIApplication application) { }

        public override void WillTerminate(UIApplication application) { }
    }
}
