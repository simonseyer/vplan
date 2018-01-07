using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FLSVertretungsplan
{
    public interface IVplanDataStore
    {
        Property<bool> IsRefreshing { get; }
        Property<bool> LastRefreshFailed { get; }
        Property<Vplan> SchoolVplan { get; }
        Property<Vplan> MyVplan { get; }
        ObservableCollection<SchoolBookmark> SchoolBookmarks { get; }
        ObservableCollection<SchoolClassBookmark> SchoolClassBookmarks { get; }
        ObservableCollection<SchoolClassBookmark> NewSchoolClassBookmarks { get; }

        Task Load();
        Task<VplanDiff> Refresh();
        Task ClearNewSchoolClassBookmarks();
        Task BookmarkSchool(string schoolName, bool bookmark);
        Task BookmarkSchoolClass(SchoolClass schoolClass, bool bookmark);
    }
}
