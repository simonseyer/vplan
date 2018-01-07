using System;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class SchoolVplanViewController : UIViewController, IVplanTabContentViewController
    {
        VplanContainerViewController VplanViewController;

        public SchoolVplanViewController() : base("SchoolVplanViewController", null)
        {
        }

        public SchoolVplanViewController(IntPtr handle) : base(handle)
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

            // Embedded vplan container
            VplanViewController = segue.DestinationViewController as VplanContainerViewController;
            VplanViewController.ViewModel = new VplanViewModel(false);
        }

        public void ResetContent()
        {
            VplanViewController.ResetContent();
        }
    }
}

