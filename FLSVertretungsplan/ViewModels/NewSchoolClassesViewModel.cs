using System;
using System.Collections.ObjectModel;

namespace FLSVertretungsplan
{
    public class NewSchoolClassesViewModel
    {
        IVplanDataStore DataStore => ServiceLocator.Instance.Get<IVplanDataStore>();

        ChipViewModel<SchoolClassBookmark> SchoolClassesPresentationModel;
        public ObservableCollection<ChipPresentationModel> SchoolClasses;

        public NewSchoolClassesViewModel()
        {
            SchoolClassesPresentationModel = new ChipViewModel<SchoolClassBookmark>(DataStore.NewSchoolClassBookmarks, ChipPresentationModel.Create);
            SchoolClasses = SchoolClassesPresentationModel.PresentationModels;
        }

        public void ToggleSchoolClassBookmarkAtIndex(int index)
        {
            var bookmark = DataStore.NewSchoolClassBookmarks[index];
            DataStore.BookmarkSchoolClass(bookmark.SchoolClass, !bookmark.Bookmarked);
        }

        public void Finish()
        {
            DataStore.ClearNewSchoolClassBookmarks();
        }
    }
}
