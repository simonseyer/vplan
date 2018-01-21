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
        SettingsViewModel ViewModel;
        ChipCollectionViewDataSource SchoolsDataSource;
        ChipCollectionViewDataSource SchoolClassesDataSource;
        ChipCollectionViewDelegate SchoolsDelegate;
        ChipCollectionViewDelegate SchoolClassesDelegate;

        public SettingsViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new SettingsViewModel();

            SchoolsDataSource = new ChipCollectionViewDataSource(SchoolCollectionView, ViewModel.Schools);
            SchoolCollectionView.DataSource = SchoolsDataSource;
            SchoolsDelegate = new ChipCollectionViewDelegate(ViewModel.Schools, ViewModel.ToggleSchoolBookmarkAtIndex);
            SchoolCollectionView.Delegate = SchoolsDelegate;

            SchoolClassesDataSource = new ChipCollectionViewDataSource(SchoolClassCollectionView, ViewModel.SchoolClasses);
            SchoolClassCollectionView.DataSource = SchoolClassesDataSource;
            SchoolClassesDelegate = new ChipCollectionViewDelegate(ViewModel.SchoolClasses, ViewModel.ToggleSchoolClassBookmarkAtIndex);
            SchoolClassCollectionView.Delegate = SchoolClassesDelegate;
            SchoolClassCollectionView.AccessibilityIdentifier = "SchoolClassCollectionView";

            TabBarItem.Title = NSBundle.MainBundle.LocalizedString("filter_tab", "");
            TitleLabel.Text = NSBundle.MainBundle.LocalizedString("filter_title", "");
            SubTitleLabel.Text = NSBundle.MainBundle.LocalizedString("filter_subtitle", "");
            SchoolSectionLabel.Text = NSBundle.MainBundle.LocalizedString("filter_school_section_title", "");
            SchoolClassSectionLabel.Text = NSBundle.MainBundle.LocalizedString("filter_school_class_section_title", "");
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
        public readonly UICollectionView CollectionView;
        public readonly ObservableCollection<ChipViewModel> ObservableItems;
        Collection<ChipViewModel> Items;

        public ChipCollectionViewDataSource(UICollectionView collectionView, ObservableCollection<ChipViewModel> items)
        {
            CollectionView = collectionView;
            ObservableItems = items;
            Items = new Collection<ChipViewModel>(ObservableItems);
            items.CollectionChanged += CollectionChanged;
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var newItems = new Collection<ChipViewModel>(ObservableItems);
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
            var cell = collectionView.DequeueReusableCell(ChipCollectionViewCell.Key, indexPath) as ChipCollectionViewCell;
            var item = Items[indexPath.Row];

            cell.Label.Text = item.Name;
            cell.Gradient = item.FillColor;
            cell.OutlineColor = item.OutlineColor.ToUIColor();
            cell.Outline = !item.Filled;

            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Items.Count;
        }
    }

    class ChipCollectionViewDelegate : UICollectionViewDelegateFlowLayout
    {
        ObservableCollection<ChipViewModel> Items;
        Action<int> ToggleBookmark;
        UISelectionFeedbackGenerator FeedbackGenerator;

        public ChipCollectionViewDelegate(ObservableCollection<ChipViewModel> items, Action<int> toggleBookmark)
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
