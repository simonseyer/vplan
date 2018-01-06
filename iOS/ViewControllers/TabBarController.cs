using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class TabBarController : UITabBarController, IUITabBarControllerDelegate
    {

        NewSchoolClassesViewModel ViewModel;
        UIViewController LastViewController;

        public TabBarController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new NewSchoolClassesViewModel();
            ViewModel.SchoolClasses.CollectionChanged += NewSchoolClasses_CollectionChanged;

            TabBar.Layer.BorderColor = UIColor.FromRGB(236, 237, 241).CGColor;
            TabBar.Layer.BorderWidth = (nfloat)0.5;
            TabBar.ClipsToBounds = true;
            TabBar.TintColor = UIColor.FromRGB(23, 43, 76);
            TabBar.UnselectedItemTintColor = UIColor.FromRGB(164, 174, 186);
            SelectedIndex = 1;
            LastViewController = SelectedViewController;

            ViewControllerSelected += (sender, e) => {
                if (LastViewController == e.ViewController)
                {
                    if (e.ViewController is IVplanTabContentViewController viewController)
                    {
                        viewController.ResetContent();
                    }
                }
                LastViewController = e.ViewController;
            };
        }

        void NewSchoolClasses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Task.Delay(200).ContinueWith(t2 =>
            {
                InvokeOnMainThread(() =>
                {
                    if (ViewModel.SchoolClasses.Count > 0 && NavigationController.PresentedViewController == null)
                    {
                        PerformSegue("showNewClasses", View);
                    }
                });
            });
        }
    }
}

