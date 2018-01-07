using System;

using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class VplanDayViewController : UIViewController
    {
        VplanDayViewModel _ViewModel;
        public VplanDayViewModel ViewModel {
            get
            {
                return _ViewModel;
            } 
            set
            {
                _ViewModel = value;
                ReloadData();
            }
        }

        VplanViewController VplanViewController;

        public VplanDayViewController() : base("VplanDayViewController", null)
        {
        }

        public VplanDayViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, Foundation.NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            VplanViewController = segue.DestinationViewController as VplanViewController;
            VplanViewController.ViewModel = ViewModel;
        }

        void ReloadData()
        {
            DayTitleLabel.Text = ViewModel?.Title;
            DaySubTitleLabel.Text = ViewModel?.SubTitle;
            VplanViewController.ViewModel = ViewModel;
        }
    }
}

