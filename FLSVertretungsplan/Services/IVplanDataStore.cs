using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FLSVertretungsplan
{
    public interface IVplanDataStore
    {
        Property<bool> IsRefreshing { get; }
        Property<Vplan> Vplan { get; }
        Property<Vplan> BookmarkedVplan { get; }
        ObservableCollection<SchoolBookmark> SchoolBookmarks { get; }
        ObservableCollection<SchoolClassBookmark> SchoolClassBookmarks { get; }
        ObservableCollection<SchoolClass> NewSchoolClasses { get; }

        Task Load();
        Task<VplanDiff> Refresh();
        Task ClearNewSchoolClasses();
        Task BookmarkSchool(string schoolName, bool bookmark);
        Task BookmarkSchoolClass(SchoolClass schoolClass, bool bookmark);
    }
}
