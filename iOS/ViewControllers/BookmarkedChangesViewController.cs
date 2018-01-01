using System;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class BookmarkedViewController : UIViewController
    {
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

            var viewController = segue.DestinationViewController as VplanContainerViewController;
            viewController.ViewModel = new VplanViewModel(true);
        }
    }
}

