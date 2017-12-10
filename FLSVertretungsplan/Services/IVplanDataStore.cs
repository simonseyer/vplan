using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FLSVertretungsplan
{
    public interface IVplanDataStore
    {
        Task<Vplan> GetVplanAsync(bool forceRefresh = false);

        Property<Vplan> GetBookmarkedVplan();
        ObservableCollection<SchoolBookmark> GetBookmarkedSchools();
        ObservableCollection<SchoolClassBookmark> GetBookmarkedClasses();

        void BookmarkSchool(string schoolName, bool bookmark);
        void BookmarkClass(SchoolClass schoolClass, bool bookmark);

    }
}
