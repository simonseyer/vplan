using System;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class SetupViewController : UIViewController
    {
        public SetupViewController() : base("SetupViewController", null)
        {
        }

        public SetupViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            StartButtonLabel.Text = NSBundle.MainBundle.LocalizedString("setup_start_button", "");
        }
    }
}

