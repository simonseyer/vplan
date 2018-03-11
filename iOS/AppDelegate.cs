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

        public override UIWindow Window { get; set; }

        bool InitialActivation = true;

        BackgroundFetchHandler BackgroundFetchHandler => new BackgroundFetchHandler();
        NotificationHandler NotificationHandler => new NotificationHandler();
        ISettingsDataStore SettingsDataStore;

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            AppCenter.Start(Secrets.IOS_APP_CENTER_SECRET, typeof(Analytics), typeof(Crashes));
            BackgroundFetchHandler.Setup();
           
            App.Initialize(false);
            SettingsDataStore = ServiceLocator.Instance.Get<ISettingsDataStore>();

            InitializeView();

            var dataStore = ServiceLocator.Instance.Get<IVplanDataStore>();
            Task.Run(() =>
            {
                dataStore.Load();
            });

            #if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
            #endif

            return true;
        }

        void InitializeView() 
        {
            var navigationController = Window.RootViewController as UINavigationController;
            var storyboard = UIStoryboard.FromName("Main", NSBundle.MainBundle);

            if (SettingsDataStore.FirstAppLaunch)
            {
                SettingsDataStore.FirstAppLaunch = false;
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
        }

        // Backdoor for tests
        [Export("activateTestEnvironment:")]
        public NSString ActivateTestEnvironment(NSString value)
        {
            var dataStore = ServiceLocator.Instance.Get<ISettingsDataStore>();
            dataStore.TestEnvironment = new NSString("true").Equals(value);

            Task.Run(() =>
            {
                _ = Reinitialize();
            });

            return new NSString();
        }

        async Task Reinitialize()
        {
            var settingsDataStore = ServiceLocator.Instance.Get<ISettingsDataStore>();
            var testEnvironment = settingsDataStore.TestEnvironment;

            App.Initialize(settingsDataStore.TestEnvironment);

            SettingsDataStore = ServiceLocator.Instance.Get<ISettingsDataStore>();
            SettingsDataStore.TestEnvironment = testEnvironment;
            SettingsDataStore.FirstAppLaunch = true;

            var dataStore = ServiceLocator.Instance.Get<IVplanDataStore>();
            await dataStore.Load();
            await dataStore.Refresh();

            InvokeOnMainThread(() =>
            {
                InitializeView();
            });
        }

        public override void OnActivated(UIApplication application)
        {
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            if (!InitialActivation)
            {
                return;
            }
            InitialActivation = false;

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
