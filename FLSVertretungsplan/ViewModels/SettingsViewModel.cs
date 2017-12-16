using System.Collections.ObjectModel;

namespace FLSVertretungsplan
{
    public class SettingsViewModel
    {
        IVplanDataStore DataStore => ServiceLocator.Instance.Get<IVplanDataStore>();
        ChipViewModel<SchoolBookmark> SchoolsPresentationModel;
        ChipViewModel<SchoolClassBookmark> SchoolClassesPresentationModel;

        public ObservableCollection<ChipPresentationModel> Schools;
        public ObservableCollection<ChipPresentationModel> SchoolClasses;

        public SettingsViewModel()
        {
            SchoolsPresentationModel = new ChipViewModel<SchoolBookmark>(DataStore.SchoolBookmarks, ChipPresentationModel.Create);
            Schools = SchoolsPresentationModel.PresentationModels;

            SchoolClassesPresentationModel = new ChipViewModel<SchoolClassBookmark>(DataStore.SchoolClassBookmarks, ChipPresentationModel.Create);
            SchoolClasses = SchoolClassesPresentationModel.PresentationModels;
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
