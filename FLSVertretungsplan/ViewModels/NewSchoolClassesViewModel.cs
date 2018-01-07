using System;
using System.Collections.ObjectModel;

namespace FLSVertretungsplan
{
    public class NewSchoolClassesViewModel
    {
        IVplanDataStore DataStore => ServiceLocator.Instance.Get<IVplanDataStore>();

        ChipCollectionViewModel<SchoolClassBookmark> CollectionViewModel;
        public readonly ObservableCollection<ChipViewModel> SchoolClasses;

        public NewSchoolClassesViewModel()
        {
            CollectionViewModel = new ChipCollectionViewModel<SchoolClassBookmark>(DataStore.NewSchoolClassBookmarks, ChipViewModel.Create);
            SchoolClasses = CollectionViewModel.ChipViewModels;
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
