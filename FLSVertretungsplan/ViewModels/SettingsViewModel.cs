using System.Collections.ObjectModel;

namespace FLSVertretungsplan
{
    public class SettingsViewModel
    {
        IVplanDataStore DataStore => ServiceLocator.Instance.Get<IVplanDataStore>();
        ChipCollectionViewModel<SchoolBookmark> SchoolCollectionViewModel;
        ChipCollectionViewModel<SchoolClassBookmark> SchoolClassCollectionViewModel;

        public readonly ObservableCollection<ChipViewModel> Schools;
        public readonly ObservableCollection<ChipViewModel> SchoolClasses;

        public SettingsViewModel()
        {
            SchoolCollectionViewModel = new ChipCollectionViewModel<SchoolBookmark>(DataStore.SchoolBookmarks, ChipViewModel.Create);
            Schools = SchoolCollectionViewModel.ChipViewModels;

            SchoolClassCollectionViewModel = new ChipCollectionViewModel<SchoolClassBookmark>(DataStore.SchoolClassBookmarks, ChipViewModel.Create);
            SchoolClasses = SchoolClassCollectionViewModel.ChipViewModels;
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
    }
}
