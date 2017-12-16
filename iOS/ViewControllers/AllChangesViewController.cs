using System;

using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class VplanViewController : UIViewController
    {
        public VplanViewController() : base("VplanViewController", null)
        {
        }

        public VplanViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, Foundation.NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            var viewController = segue.DestinationViewController as BrowseViewController;
            viewController.bookmarkedVplan = false;
        }
    }
}

