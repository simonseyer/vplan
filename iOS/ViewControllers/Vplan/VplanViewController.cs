using System;
using Foundation;
using UIKit;
using System.Collections.ObjectModel;
using CoreGraphics;

namespace FLSVertretungsplan.iOS
{
    public partial class VplanViewController : UITableViewController
    {
        UIRefreshControl TableViewRefreshControl;
        UILongPressGestureRecognizer LongPressGestureRecognizer;

        VplanDayViewModel _ViewModel;
        public VplanDayViewModel ViewModel
        {
            get
            {
                return _ViewModel;
            }
            set
            {
                if (_ViewModel != null)
                {
                    _ViewModel.IsRefreshing.PropertyChanged -= IsRefreshing_PropertyChanged;
                }
                _ViewModel = value;
                if (_ViewModel != null)
                {
                    _ViewModel.IsRefreshing.PropertyChanged += IsRefreshing_PropertyChanged;
                }
                ReloadData();
            }
        }

        ItemsDataSource DataSource = new ItemsDataSource();

        public VplanViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableViewRefreshControl = new UIRefreshControl();
            TableViewRefreshControl.Layer.ZPosition = -1;
            TableViewRefreshControl.ValueChanged += RefreshControl_ValueChanged;
            TableView.RefreshControl = TableViewRefreshControl;
            TableView.Source = DataSource;

            TableView.ClipsToBounds = true;
            TableView.Layer.CornerRadius = 6;
            TableView.Layer.BorderWidth = 0.5F;
            TableView.Layer.BorderColor = Color.TAB_BORDER_COLOR.ToUIColor().CGColor;
            TableView.ContentInset = new UIEdgeInsets(14, 0, 14, 0);

            TableView.RegisterNibForCellReuse(ChangeTableViewCell.Nib, ChangeTableViewCell.Key);

            AddLongPressGestureRecognizerIfNeeded();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            AddLongPressGestureRecognizerIfNeeded();
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            ViewModel.LoadItemsCommand.Execute(null);
        }

        void IsRefreshing_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                if (ViewModel.IsRefreshing.Value && !TableViewRefreshControl.Refreshing)
                {
                    TableViewRefreshControl.BeginRefreshing();
                }
                else if (!ViewModel.IsRefreshing.Value)
                {
                    TableViewRefreshControl.EndRefreshing();
                }
            });
        }

        void ReloadData()
        {
            InvokeOnMainThread(() =>
            {
                DataSource.Context = this;
                DataSource.Items = ViewModel?.Items;
                TableView.ReloadData();
                var lastUpdateText = NSBundle.MainBundle.LocalizedString("vplan_last_update", ""); 
                LastUpdateLabel.Text = NSString.LocalizedFormat(lastUpdateText, ViewModel?.LastUpdate);
            });
        }

        void AddLongPressGestureRecognizerIfNeeded()
        {
            if (TraitCollection.ForceTouchCapability == UIForceTouchCapability.Unavailable &&
                LongPressGestureRecognizer == null)
            {
                LongPressGestureRecognizer = new UILongPressGestureRecognizer();
                LongPressGestureRecognizer.AddTarget(() => this.LongPressPerformed(LongPressGestureRecognizer));
                TableView.AddGestureRecognizer(LongPressGestureRecognizer);
            }
        }

        void LongPressPerformed(UILongPressGestureRecognizer gestureRecognizer)
        {
            if (gestureRecognizer.State != UIGestureRecognizerState.Began)
            {
                return;
            }

            var location = gestureRecognizer.LocationInView(TableView);
            var indexPath = TableView.IndexPathForRowAtPoint(location);
            if (indexPath != null)
            {
                var shareViewController = new ChangeViewController
                {
                    ViewModel = DataSource.Items[indexPath.Row],
                    Context = this
                };
                var alertController = new UIAlertController();
                alertController.AddAction(UIAlertAction.Create(NSBundle.MainBundle.LocalizedString("share_image", ""), UIAlertActionStyle.Default,(obj) => 
                {
                    shareViewController.ShareImage();
                }));
                alertController.AddAction(UIAlertAction.Create(NSBundle.MainBundle.LocalizedString("share_text", ""), UIAlertActionStyle.Default, (obj) =>
                {
                    shareViewController.ShareText();
                }));
                alertController.AddAction(UIAlertAction.Create(NSBundle.MainBundle.LocalizedString("share_calendar_entry", ""), UIAlertActionStyle.Default, (obj) =>
                {
                    shareViewController.ShareCalendarEvent();
                }));
                alertController.AddAction(UIAlertAction.Create(NSBundle.MainBundle.LocalizedString("share_done", ""), UIAlertActionStyle.Cancel, (obj) => {}));
                PresentViewController(alertController, true, null);
            }
        }
    }

    partial class VplanViewController: IUIViewControllerPreviewingDelegate
    {
        public UIViewController GetViewControllerForPreview(IUIViewControllerPreviewing previewingContext, CGPoint location)
        {
            var cellPosition = TableView.ConvertPointFromView(location, previewingContext.SourceView);
            var indexPath = TableView.IndexPathForRowAtPoint(cellPosition);
            if (indexPath != null)
            {
                var viewController = new ChangeViewController
                {
                    ViewModel = DataSource.Items[indexPath.Row],
                    Context = this
                };
                var tableCell = TableView.CellAt(indexPath);
                previewingContext.SourceRect = View.ConvertRectFromView(tableCell.ContentView.Frame, TableView);

                return viewController;
            }
            return null;
        }

        public void CommitViewController(IUIViewControllerPreviewing previewingContext, UIViewController viewControllerToCommit) { }
    }

    class ItemsDataSource : UITableViewSource
    {
        public ReadOnlyCollection<ChangeViewModel> Items;
        public VplanViewController Context;

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section) 
        {
            return Items?.Count ?? 0;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            ChangeTableViewCell cell = (ChangeTableViewCell)tableView.DequeueReusableCell(ChangeTableViewCell.Key, indexPath);
            var change = Items[indexPath.Row];

            cell.SchoolClassLabel.Text = change.ClassName;
            cell.HoursLabel.Text = change.Hours;
            cell.ChangeLabel.Text = NSBundle.MainBundle.LocalizedString(change.Type, "");
            cell.SchoolGradientView.Gradient = change.FillColor;
            cell.OriginalLessonLabel.AttributedText = TextComponentFormatter.AttributedStringForTextComponents(change.OldLesson, true);
            cell.ChangeDescriptionLabel.AttributedText = TextComponentFormatter.AttributedStringForTextComponents(change.Description, false);

            if (Context.TraitCollection.ForceTouchCapability == UIForceTouchCapability.Available)
            {
                Context.RegisterForPreviewingWithDelegate(Context, cell.ContentView);
            }

            return cell;
        }
    }
}
