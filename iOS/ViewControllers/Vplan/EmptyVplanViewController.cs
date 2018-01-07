using System;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class EmptyVplanViewController : UIViewController
    {
        VplanViewModel _ViewModel;
        public VplanViewModel ViewModel { 
            get
            {
                return _ViewModel;
            }
            set
            {
                if (ViewModel != null)
                {
                    ViewModel.IsRefreshing.PropertyChanged -= IsRefreshing_PropertyChanged;
                }
                _ViewModel = value;
                if (ViewModel != null)
                {
                    ViewModel.IsRefreshing.PropertyChanged += IsRefreshing_PropertyChanged;
                }
            }
        }
        UIRefreshControl RefreshControl;

        public EmptyVplanViewController() : base("EmptyVplanViewController", null)
        {
        }

        public EmptyVplanViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            RefreshControl = new UIRefreshControl();
            RefreshControl.ValueChanged += RefreshControl_ValueChanged;
            ScrollView.AddSubview(RefreshControl);

            TitleLabel.Text = NSBundle.MainBundle.LocalizedString("empty_vplan_title", "");
            SubTitleLabel.Text = NSBundle.MainBundle.LocalizedString("empty_vplan_subtitle", "");
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            ViewModel.LoadItemsCommand.Execute(null);
        }

        void IsRefreshing_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                if (ViewModel.IsRefreshing.Value && !RefreshControl.Refreshing)
                {
                    RefreshControl.BeginRefreshing();
                }
                else if (!ViewModel.IsRefreshing.Value)
                {
                    RefreshControl.EndRefreshing();
                }
            });
        }
    }
}

