using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FLSVertretungsplan
{
    public interface IVplanDataStore
    {
        Task Refresh();

        Property<bool> GetIsRefreshing();
        Property<Vplan> GetVplan();
        Property<Vplan> GetBookmarkedVplan();
        ObservableCollection<SchoolBookmark> GetSchoolBookmarks();
        ObservableCollection<SchoolClassBookmark> GetSchoolClassBookmarks();

        void BookmarkSchool(string schoolName, bool bookmark);
        void BookmarkSchoolClass(SchoolClass schoolClass, bool bookmark);

    }
}
