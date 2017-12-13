using System;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public class ChipPresentationModel
    {
        public string Name { get; private set; }
        public int Color { get; private set; }
        public bool Visible { get; private set; }

        private static readonly Dictionary<string, int> Colors = new Dictionary<string, int>
        {
            { "BG", 0x5d94cc },
            { "BS", 0x73db43 },
            { "BFS", 0xdd7b35 },
            { "HBFS", 0x1ec9a4 }
        };

        internal ChipPresentationModel(SchoolBookmark bookmark)
        {
            Name = bookmark.School;
            Color = bookmark.Bookmarked ? GetColor(bookmark.School) : 0xdbdcdd;
            Visible = true;
        }

        internal ChipPresentationModel(SchoolClassBookmark bookmark)
        {
            Name = bookmark.SchoolClass.Name;
            Color = bookmark.Bookmarked ? GetColor(bookmark.SchoolClass.School) : 0xdbdcdd;
            Visible = bookmark.SchoolBookmarked;
        }

        private int GetColor(string school) {
            if (!Colors.ContainsKey(school))
            {
                return 0xc91d1d;
            }
            return Colors[school];
        }
    }

    public class SettingsViewModel
    {
        IVplanDataStore DataStore => ServiceLocator.Instance.Get<IVplanDataStore>();
        public ObservableCollection<ChipPresentationModel> Schools;
        public ObservableCollection<ChipPresentationModel> SchoolClasses;

        public SettingsViewModel()
        {
            Schools = new ObservableCollection<ChipPresentationModel>();
            LoadSchools();
            DataStore.SchoolBookmarks.CollectionChanged += Schools_CollectionChanged;

            SchoolClasses = new ObservableCollection<ChipPresentationModel>();
            LoadSchoolClasses();
            DataStore.SchoolClassBookmarks.CollectionChanged += SchoolClasses_CollectionChanged;
        }

        public void ToggleSchoolBookmarkAtIndex(int index)
        {
            var bookmark = DataStore.SchoolBookmarks[index];
            DataStore.BookmarkSchool(bookmark.School, !bookmark.Bookmarked);
        }

        public void ToggleSchoolClassBookmarkAtIndex(int index)
        {
            var bookmark = DataStore.SchoolClassBookmarks[index];
            DataStore.BookmarkSchoolClass(bookmark.SchoolClass, !bookmark.Bookmarked);
        }

        private void LoadSchools()
        {
            Schools.Clear();
            foreach (SchoolBookmark bookmark in DataStore.SchoolBookmarks)
            {
                Schools.Add(new ChipPresentationModel(bookmark));
            }
        }

        private void Schools_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        SchoolBookmark bookmark = e.NewItems[i] as SchoolBookmark;
                        var newPresentationModel = new ChipPresentationModel(bookmark);
                        Schools.Insert(e.NewStartingIndex + i, newPresentationModel);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        Schools.RemoveAt(e.OldStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        SchoolBookmark bookmark = e.NewItems[i] as SchoolBookmark;
                        var newPresentationModel = new ChipPresentationModel(bookmark);
                        Schools[e.NewStartingIndex + i] = newPresentationModel;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotSupportedException("Moving Schools is not supported");
                case NotifyCollectionChangedAction.Reset:
                    LoadSchools();
                    break;
            }
        }

        private void LoadSchoolClasses()
        {
            SchoolClasses.Clear();

            var bookmarks = DataStore.SchoolClassBookmarks;
            for (var i = 0; i < bookmarks.Count; i++)
            {
                if (bookmarks[i].SchoolBookmarked)
                {
                    SchoolClasses.Add(new ChipPresentationModel(bookmarks[i]));
                }
            }
        }

        private void SchoolClasses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        SchoolClassBookmark bookmark = e.NewItems[i] as SchoolClassBookmark;
                        var newPresentationModel = new ChipPresentationModel(bookmark);
                        SchoolClasses.Insert(e.NewStartingIndex + i, newPresentationModel);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    for (var i = 0; i < e.OldItems.Count; i++)
                    {
                        SchoolClasses.RemoveAt(e.OldStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        var newBookmark = e.NewItems[i] as SchoolClassBookmark;
                        var newPresentationModel = new ChipPresentationModel(newBookmark);
                        SchoolClasses[e.NewStartingIndex + i] = newPresentationModel;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotSupportedException("Moving SchoolClasses is not supported");
                case NotifyCollectionChangedAction.Reset:
                    LoadSchoolClasses();
                    break;
            }
        }
    }
}
