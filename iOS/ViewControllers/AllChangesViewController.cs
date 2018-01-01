using System;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class AllChangesViewController : UIViewController
    {
        public AllChangesViewController() : base("VplanViewController", null)
        {
        }

        public AllChangesViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TabBarItem.Title = NSBundle.MainBundle.LocalizedString("school_tab", "");
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, Foundation.NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            var viewController = segue.DestinationViewController as VplanContainerViewController;
            viewController.ViewModel = new VplanViewModel(false);
        }
    }
}

