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

        static TabBarController()
        {
            UITabBarItem.Appearance.SetTitleTextAttributes(new UITextAttributes
            {
                TextColor = Color.ICON_COLOR.ToUIColor()
            }, UIControlState.Normal);

            UITabBarItem.Appearance.SetTitleTextAttributes(new UITextAttributes
            {
                TextColor = Color.SELECTED_ICON_COLOR.ToUIColor()
            }, UIControlState.Selected);
        }

        public TabBarController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new NewSchoolClassesViewModel();
            ViewModel.SchoolClasses.CollectionChanged += NewSchoolClasses_CollectionChanged;

            TabBar.Layer.BorderColor = Color.TAB_BORDER_COLOR.ToUIColor().CGColor;
            TabBar.Layer.BorderWidth = (nfloat)0.5;
            TabBar.ClipsToBounds = true;
            TabBar.TintColor = Color.SELECTED_ICON_COLOR.ToUIColor();
            TabBar.UnselectedItemTintColor = Color.ICON_COLOR.ToUIColor();
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

