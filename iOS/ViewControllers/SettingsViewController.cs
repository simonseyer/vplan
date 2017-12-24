using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CoreGraphics;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class SettingsViewController : UIViewController
    {
        SettingsViewModel ViewModel { get; set; }
        ChipCollectionViewDataSource SchoolsDataSource { get; set; }
        ChipCollectionViewDataSource SchoolClassesDataSource { get; set; }
        ChipCollectionViewDelegate SchoolsDelegate { get; set; }
        ChipCollectionViewDelegate SchoolClassesDelegate { get; set; }

        public SettingsViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new SettingsViewModel();

            SchoolsDataSource = new ChipCollectionViewDataSource(schoolCollectionView, ViewModel.Schools);
            schoolCollectionView.DataSource = SchoolsDataSource;
            SchoolsDelegate = new ChipCollectionViewDelegate(ViewModel.Schools, ViewModel.ToggleSchoolBookmarkAtIndex);
            schoolCollectionView.Delegate = SchoolsDelegate;

            SchoolClassesDataSource = new ChipCollectionViewDataSource(classCollectionView, ViewModel.SchoolClasses);
            classCollectionView.DataSource = SchoolClassesDataSource;
            SchoolClassesDelegate = new ChipCollectionViewDelegate(ViewModel.SchoolClasses, ViewModel.ToggleSchoolClassBookmarkAtIndex);
            classCollectionView.Delegate = SchoolClassesDelegate;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SchoolsDelegate.ActivateFeedback(true);
            SchoolClassesDelegate.ActivateFeedback(true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            SchoolsDelegate.ActivateFeedback(false);
            SchoolClassesDelegate.ActivateFeedback(false);
        }
    }

    class ChipCollectionViewDataSource : UICollectionViewDataSource
    {

        public UICollectionView CollectionView { get; private set; }
        public ObservableCollection<ChipPresentationModel> ObservableItems { get; private set; }
        public Collection<ChipPresentationModel> Items { get; private set; }

        public ChipCollectionViewDataSource(UICollectionView collectionView, ObservableCollection<ChipPresentationModel> items)
        {
            CollectionView = collectionView;
            ObservableItems = items;
            Items = new Collection<ChipPresentationModel>(ObservableItems);
            items.CollectionChanged += CollectionChanged;
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var newItems = new Collection<ChipPresentationModel>(ObservableItems);
            InvokeOnMainThread(() =>
            {
                Items = newItems;
                var oldIndexPaths = GetIndexPaths(e.OldStartingIndex, e.OldItems?.Count ?? 0);
                var newIndexPaths = GetIndexPaths(e.NewStartingIndex, e.NewItems?.Count ?? 0);
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        CollectionView.InsertItems(newIndexPaths);
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        CollectionView.DeleteItems(oldIndexPaths);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        CollectionView.ReloadItems(newIndexPaths);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        throw new NotSupportedException("Moving items is not supported");
                    case NotifyCollectionChangedAction.Reset:
                        CollectionView.ReloadData();
                        break;
                }
            });
        }

        NSIndexPath[] GetIndexPaths(int startIndex, int count)
        {
            var indexPaths = new NSIndexPath[count];
            for (var i = 0; i < count; i++)
            {
                indexPaths[i] = NSIndexPath.FromRowSection((nint)(startIndex + i), 0);
            }
            return indexPaths;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell("CHIP_CELL", indexPath) as ChipCell;
            var item = Items[indexPath.Row];

            cell.Label.Text = item.Name;
            if (item.Filled)
            {
                cell.Gradient = item.FillColor;
                cell.Layer.BorderWidth = 0;
                cell.Layer.BorderColor = UIColor.Clear.CGColor;
                cell.Label.TextColor = UIColor.White;
            }
            else
            {
                cell.Gradient = null;
                cell.Layer.BorderWidth = 1;
                cell.Layer.BorderColor = item.OutlineColor.ToUIColor().CGColor;
                cell.Label.TextColor = item.OutlineColor.ToUIColor();
            }


            cell.Layer.CornerRadius = 18;
            cell.ClipsToBounds = true;

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Items.Count;
        }

    }

    class ChipCollectionViewDelegate : UICollectionViewDelegateFlowLayout
    {
        ObservableCollection<ChipPresentationModel> Items;
        Action<int> ToggleBookmark;
        UISelectionFeedbackGenerator FeedbackGenerator;

        public ChipCollectionViewDelegate(ObservableCollection<ChipPresentationModel> items, Action<int> toggleBookmark)
        {
            Items = items;
            ToggleBookmark = toggleBookmark;
        }

        public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var name = new NSString(Items[indexPath.Row].Name);
            var size = name.GetSizeUsingAttributes(new UIStringAttributes() { Font = UIFont.SystemFontOfSize(17) });
            return new CGSize(size.Width + 24, 36);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ToggleBookmark.Invoke(indexPath.Row);
            FeedbackGenerator?.SelectionChanged();
            FeedbackGenerator?.Prepare();
        }

        public void ActivateFeedback(bool activate)
        {
            if (activate)
            {
                FeedbackGenerator = new UISelectionFeedbackGenerator();
                FeedbackGenerator.Prepare();
            }
            else
            {
                FeedbackGenerator = null;
            }
        }

    }


}

