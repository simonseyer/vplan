using System;
using System.Collections.Specialized;

using Foundation;
using UIKit;
using FLSVertretungsplan.iOS.Views;

namespace FLSVertretungsplan.iOS
{
    public partial class BrowseViewController : UITableViewController
    {
        UIRefreshControl refreshControl;

        public VplanViewModel ViewModel { get; set; }
        public bool bookmarkedVplan;

        public BrowseViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new VplanViewModel(bookmarkedVplan);

            // Setup UITableView.
            refreshControl = new UIRefreshControl();
            refreshControl.ValueChanged += RefreshControl_ValueChanged;
            TableView.RefreshControl = refreshControl;
            TableView.Source = new ItemsDataSource(ViewModel);

            LastUpdateLabel.Text = ViewModel.LastUpdate.Value;

            ViewModel.IsRefreshing.PropertyChanged += IsRefreshing_PropertyChanged;
            ViewModel.LastUpdate.PropertyChanged += LastUpdate_PropertyChanged;
            ViewModel.Dates.PropertyChanged += Dates_CollectionChanged;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            ViewModel.LoadItemsCommand.Execute(null);
        }

        void IsRefreshing_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                if (ViewModel.IsRefreshing.Value && !refreshControl.Refreshing)
                {
                    refreshControl.BeginRefreshing();
                }
                else if (!ViewModel.IsRefreshing.Value)
                {
                    refreshControl.EndRefreshing();
                }
            });
        }

        void LastUpdate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                LastUpdateLabel.Text = ViewModel.LastUpdate.Value;
            });
        }

        void Dates_CollectionChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                TableView.ReloadData();
            });
        }
    }

    class ItemsDataSource : UITableViewSource
    {
        static readonly NSString CELL_IDENTIFIER = new NSString("CHANGE_CELL");

        VplanViewModel viewModel;

        public ItemsDataSource(VplanViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return viewModel.Dates.Value.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section) 
        {
            return viewModel.Dates.Value[(int)section].Items.Count;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return viewModel.Dates.Value[(int)section].Title;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            ChangeCell cell = (ChangeCell)tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath);

            cell.StatusView.Layer.CornerRadius = 6; // TODO

            var change = viewModel.Dates.Value[indexPath.Section].Items[indexPath.Row];

            cell.ClassNameLabel.Text = change.ClassName;
            cell.HoursLabel.Text = change.Hours;
            cell.OriginalLessonLabel.Text = change.OldLesson;
            cell.ChangeTypeLabel.Text = change.Type;
            cell.ChangeDescriptionLabel.Text = change.Description;
            cell.StatusView.BackgroundColor = change.Canceled ? UIColor.FromRGB(0.76f, 0.19f, 0.12f) : UIColor.FromRGB(0.18f, 0.76f, 0.23f);

            return cell;
        }
    }
}
