using System;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class MyVplanViewController : UIViewController, IVplanTabContentViewController
    {

        VplanContainerViewController VplanViewController;

        public MyVplanViewController() : base("MyVplanViewController", null)
        {
        }

        public MyVplanViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TabBarItem.Title = NSBundle.MainBundle.LocalizedString("my_plan_tab", "");
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, Foundation.NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            // Embedded vplan container
            VplanViewController = segue.DestinationViewController as VplanContainerViewController;
            VplanViewController.ViewModel = new VplanViewModel(true);
        }

        public void ResetContent()
        {
            VplanViewController.ResetContent();
        }
    }
}

