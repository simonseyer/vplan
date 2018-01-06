using System;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class BookmarkedViewController : UIViewController, IVplanTabContentViewController
    {

        VplanContainerViewController VplanViewController;

        public BookmarkedViewController() : base("BookmarkedViewController", null)
        {
        }

        public BookmarkedViewController(IntPtr handle) : base(handle)
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

            VplanViewController = segue.DestinationViewController as VplanContainerViewController;
            VplanViewController.ViewModel = new VplanViewModel(true);
        }

        public void ResetContent()
        {
            VplanViewController.ResetContent();
        }
    }
}

