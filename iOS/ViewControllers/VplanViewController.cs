using System;
using Foundation;
using UIKit;
using System.Collections.ObjectModel;
using CoreGraphics;

namespace FLSVertretungsplan.iOS
{
    public partial class VplanViewController : UITableViewController
    {
        UIRefreshControl refreshControl;

        DatePresentationModel _PresentationModel;
        public DatePresentationModel PresentationModel
        {
            get
            {
                return _PresentationModel;
            }
            set
            {
                if (_PresentationModel != null)
                {
                    _PresentationModel.IsRefreshing.PropertyChanged -= IsRefreshing_PropertyChanged;
                }
                _PresentationModel = value;
                if (_PresentationModel != null)
                {
                    _PresentationModel.IsRefreshing.PropertyChanged += IsRefreshing_PropertyChanged;
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

            refreshControl = new UIRefreshControl();
            refreshControl.Layer.ZPosition = -1;
            refreshControl.ValueChanged += RefreshControl_ValueChanged;
            TableView.RefreshControl = refreshControl;
            TableView.Source = DataSource;

            TableView.ClipsToBounds = true;
            TableView.Layer.CornerRadius = 6;
            TableView.Layer.BorderWidth = 0.5F;
            TableView.Layer.BorderColor = UIColor.FromRGB(236, 237, 241).CGColor;
            TableView.ContentInset = new UIEdgeInsets(14, 0, 14, 0);

            TableView.RegisterNibForCellReuse(ChangeTableViewCell.Nib, ChangeTableViewCell.Key);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            PresentationModel.LoadItemsCommand.Execute(null);
        }

        void IsRefreshing_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                if (PresentationModel.IsRefreshing.Value && !refreshControl.Refreshing)
                {
                    refreshControl.BeginRefreshing();
                }
                else if (!PresentationModel.IsRefreshing.Value)
                {
                    refreshControl.EndRefreshing();
                }
            });
        }

        void ReloadData()
        {
            InvokeOnMainThread(() =>
            {
                DataSource.Context = this;
                DataSource.Items = PresentationModel?.Items;
                TableView.ReloadData();
                var lastUpdateText = NSBundle.MainBundle.LocalizedString("vplan_last_update", ""); 
                LastUpdateLabel.Text = NSString.LocalizedFormat(lastUpdateText, PresentationModel?.LastUpdate);
            });
        }

        bool IsForceTouchAvailbale()
        {
            return TraitCollection.ForceTouchCapability == UIForceTouchCapability.Available;
        }
    }

    class ItemsDataSource : UITableViewSource, IUIViewControllerPreviewingDelegate
    {
        public Collection<ChangePresentationModel> Items;
        public UITableViewController Context;

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

            Context.RegisterForPreviewingWithDelegate(this, cell.ContentView);

            return cell;
        }

        public UIViewController GetViewControllerForPreview(IUIViewControllerPreviewing previewingContext, CGPoint location)
        {
            var cellPosition = Context.TableView.ConvertPointFromView(location, previewingContext.SourceView);
            var indexPath = Context.TableView.IndexPathForRowAtPoint(cellPosition);
            if (indexPath != null)
            {
                var viewController = new ChangeViewController();
                viewController.SetPresentationModel(Items[indexPath.Row]);
                viewController.Context = Context;

                var tableCell = Context.TableView.CellAt(indexPath);
                previewingContext.SourceRect = Context.View.ConvertRectFromView(tableCell.ContentView.Frame, Context.TableView);

                return viewController;
            }
            return null;
        }

        public void CommitViewController(IUIViewControllerPreviewing previewingContext, UIViewController viewControllerToCommit)
        {

        }
    }
}
