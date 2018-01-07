using System;
using Foundation;
using UIKit;
using UserNotifications;

namespace FLSVertretungsplan.iOS
{
    public partial class SetupViewController : UIViewController
    {
        public NotificationHandler NotificationHandler;

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

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            NotificationHandler.ActivateNotifications();
        }
    }
}
