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

        public ItemsViewModel ViewModel { get; set; }

        public BrowseViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            NavigationController.NavigationBar.PrefersLargeTitles = true;

            base.ViewDidLoad();

            ViewModel = new ItemsViewModel();

            // Setup UITableView.
            refreshControl = new UIRefreshControl();
            refreshControl.ValueChanged += RefreshControl_ValueChanged;
            TableView.RefreshControl = refreshControl;
            TableView.Source = new ItemsDataSource(ViewModel);

            Title = ViewModel.Title;

            ViewModel.PropertyChanged += IsBusy_PropertyChanged;
            ViewModel.Changes.CollectionChanged += Items_CollectionChanged;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (ViewModel.Changes.Count == 0)
                ViewModel.LoadItemsCommand.Execute(null);
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            if (!ViewModel.IsBusy && refreshControl.Refreshing)
                ViewModel.LoadItemsCommand.Execute(null);
        }

        void IsBusy_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;
            switch (propertyName)
            {
                case nameof(ViewModel.IsBusy):
                    {
                        InvokeOnMainThread(() =>
                        {
                            if (ViewModel.IsBusy && !refreshControl.Refreshing)
                            {
                                refreshControl.BeginRefreshing();
                            }
                            else if (!ViewModel.IsBusy)
                            {
                                refreshControl.EndRefreshing();
                                TableView.ReloadData();
                            }
                        });
                    }
                    break;
                case nameof(ViewModel.LastUpdate):
                    {
                        InvokeOnMainThread(() =>
                        {
                            LastUpdateLabel.Text = ViewModel.LastUpdate;
                        });
                    }
                    break;
            }
        }

        void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }
    }

    class ItemsDataSource : UITableViewSource
    {
        static readonly NSString CELL_IDENTIFIER = new NSString("CHANGE_CELL");

        ItemsViewModel viewModel;

        public ItemsDataSource(ItemsViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return viewModel.Dates.Count;
        }

        public override nint RowsInSection(UITableView tableview, nint section) 
        {
            return viewModel.Changes[(int)section].Count;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return viewModel.Dates[(int)section];
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            ChangeCell cell = (ChangeCell)tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath);

            cell.StatusView.Layer.CornerRadius = 6; // TODO

            var change = viewModel.Changes[indexPath.Section][indexPath.Row];

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
