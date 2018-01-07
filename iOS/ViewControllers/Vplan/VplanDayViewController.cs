using System;

using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class VplanDayViewController : UIViewController
    {
        DatePresentationModel _PresentationModel;
        public DatePresentationModel PresentationModel {
            get
            {
                return _PresentationModel;
            } 
            set
            {
                _PresentationModel = value;
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
            VplanViewController.PresentationModel = _PresentationModel;
        }

        void ReloadData()
        {
            DayTitleLabel.Text = _PresentationModel?.Title;
            DaySubTitleLabel.Text = _PresentationModel?.SubTitle;
            VplanViewController.PresentationModel = _PresentationModel;
        }
    }
}

