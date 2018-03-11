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
        ChipCollectionViewDataSource DataSource;
        ChipCollectionViewDelegate Delegate;

        public SettingsViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel = new SettingsViewModel();

            var sections = new Collection<ChipSection>()
            {
                new ChipSection(
                    NSBundle.MainBundle.LocalizedString("filter_school_section_title", ""), 
                    ViewModel.Schools, 
                    ViewModel.ToggleSchoolBookmarkAtIndex
                ),
                new ChipSection(
                    NSBundle.MainBundle.LocalizedString("filter_school_class_section_title", ""), 
                    ViewModel.SchoolClasses, 
                    ViewModel.ToggleSchoolClassBookmarkAtIndex
                )
            };
            DataSource = new ChipCollectionViewDataSource(CollectionView, sections);
            CollectionView.DataSource = DataSource;
            Delegate = new ChipCollectionViewDelegate(sections);
            CollectionView.Delegate = Delegate;
            CollectionView.AccessibilityIdentifier = "filterCollectionView";

            TabBarItem.Title = NSBundle.MainBundle.LocalizedString("filter_tab", "");
            TitleLabel.Text = NSBundle.MainBundle.LocalizedString("filter_title", "");
            SubTitleLabel.Text = NSBundle.MainBundle.LocalizedString("filter_subtitle", "");
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Delegate.ActivateFeedback(true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            Delegate.ActivateFeedback(false);
        }
    }

    class ChipSection 
    {
        public readonly string Title;
        public readonly ObservableCollection<ChipViewModel> Items;
        public readonly Action<int> ToggleAction;

        public ChipSection(string title, ObservableCollection<ChipViewModel> items, Action<int> toggleAction)
        {
            Title = title;
            Items = items;
            ToggleAction = toggleAction;
        }
    }

    class ChipCollectionViewDataSource : UICollectionViewDataSource
    {
        readonly UICollectionView CollectionView;
        readonly Collection<ChipSection> Sections;
        Collection<Collection<ChipViewModel>> Items;
        readonly Collection<Binding> Bindings;

        public ChipCollectionViewDataSource(UICollectionView collectionView, Collection<ChipSection> sections)
        {
            CollectionView = collectionView;
            Sections = sections;
            Items = new Collection<Collection<ChipViewModel>>();
            Bindings = new Collection<Binding>();

            int sectionIndex = 0;
            foreach (ChipSection section in sections)
            {
                Items.Add(section.Items);
                Bindings.Add(new Binding(sectionIndex, this));
                sectionIndex += 1;
            }
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = collectionView.DequeueReusableCell(ChipCollectionViewCell.Key, indexPath) as ChipCollectionViewCell;
            var item = Items[indexPath.Section][indexPath.Row];

            cell.Label.Text = item.Name;
            cell.Gradient = item.FillColor;
            cell.OutlineColor = item.OutlineColor.ToUIColor();
            cell.Outline = !item.Filled;

            return cell;
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            var view = collectionView.DequeueReusableSupplementaryView(elementKind, ChipCollectionHeaderView.Key, indexPath) as ChipCollectionHeaderView;

            view.TitleLabel.Text = Sections[indexPath.Section].Title;
            if (UIScreen.MainScreen.Bounds.Height < 600)
            {
                view.TitleTopMarginConstraint.Constant = 10;
            }

            return view;
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return Items.Count;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return Items[(int)section].Count;
        }

        class Binding
        {
            readonly int Section;
            readonly ChipCollectionViewDataSource DataSource;

            public Binding(int section, ChipCollectionViewDataSource dataSource)
            {
                Section = section;
                DataSource = dataSource;
                DataSource.Sections[section].Items.CollectionChanged += CollectionChanged;
            }

            void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                var newItems = new Collection<ChipViewModel>(DataSource.Sections[Section].Items);
                DataSource.InvokeOnMainThread(() =>
                {
                    DataSource.Items[Section] = newItems;
                    var oldIndexPaths = GetIndexPaths(e.OldStartingIndex, e.OldItems?.Count ?? 0);
                    var newIndexPaths = GetIndexPaths(e.NewStartingIndex, e.NewItems?.Count ?? 0);
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            DataSource.CollectionView.InsertItems(newIndexPaths);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            DataSource.CollectionView.DeleteItems(oldIndexPaths);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            DataSource.CollectionView.ReloadItems(newIndexPaths);
                            break;
                        case NotifyCollectionChangedAction.Move:
                            throw new NotSupportedException("Moving items is not supported");
                        case NotifyCollectionChangedAction.Reset:
                            DataSource.CollectionView.ReloadData();
                            break;
                    }
                });
            }

            NSIndexPath[] GetIndexPaths(int startIndex, int count)
            {
                var indexPaths = new NSIndexPath[count];
                for (var i = 0; i < count; i++)
                {
                    indexPaths[i] = NSIndexPath.FromRowSection(startIndex + i, Section);
                }
                return indexPaths;
            }
        }
    }

    class ChipCollectionViewDelegate : UICollectionViewDelegateFlowLayout
    {
        readonly Collection<ChipSection> Sections;
        UISelectionFeedbackGenerator FeedbackGenerator;

        public ChipCollectionViewDelegate(Collection<ChipSection> sections)
        {
            Sections = sections;
        }

        public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var name = new NSString(Sections[indexPath.Section].Items[indexPath.Row].Name);
            var size = name.GetSizeUsingAttributes(new UIStringAttributes() { Font = UIFont.SystemFontOfSize(17) });
            return new CGSize(size.Width + 24, 36);
        }

        public override CGSize GetReferenceSizeForHeader(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            if (Sections[(int)section].Title == null)
            {
                return new CGSize(0, 0);
            }

            var baseHeight = 26.5 + 3 + 20 + 10;
            if (UIScreen.MainScreen.Bounds.Height >= 600)
            {
                baseHeight += 30;
            }
            return new CGSize(0, baseHeight);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            Sections[indexPath.Section].ToggleAction.Invoke(indexPath.Row);
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
