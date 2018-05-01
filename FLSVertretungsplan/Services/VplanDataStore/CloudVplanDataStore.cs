using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Collections.Immutable;

namespace FLSVertretungsplan
{
    public class CloudVplanDataStore: IVplanDataStore
    {

        IVplanLoader Loader => ServiceLocator.Instance.Get<IVplanLoader>();
        IVplanPersistence Persistence => ServiceLocator.Instance.Get<IVplanPersistence>();
        IDefaultData DefaultData => ServiceLocator.Instance.Get<IDefaultData>();

        // Transient state
        public Property<bool> IsRefreshing { get; }
        public Property<bool> LastRefreshFailed { get; }

        // State
        public Property<Vplan> SchoolVplan { get; }
        public HashSet<SchoolClassBookmark> AllSchoolClassBookmarks { get; }
        public ObservableCollection<SchoolBookmark> SchoolBookmarks { get; }
        public ObservableCollection<SchoolClassBookmark> NewSchoolClassBookmarks { get; }

        // Derived state
        public Property<Vplan> MyVplan { get; }
        // Contains only SchoolClassBookmarks of the bookmarked schools
        public ObservableCollection<SchoolClassBookmark> SchoolClassBookmarks { get; }

        Task LoadTask;
        Task<VplanDiff> RefreshTask;
        bool Loaded;

        public CloudVplanDataStore()
        {
            IsRefreshing = new Property<bool>();
            LastRefreshFailed = new Property<bool>();
            SchoolVplan = new Property<Vplan> { Value = null };
            MyVplan = new Property<Vplan>() { Value = null };
            SchoolBookmarks = new ObservableCollection<SchoolBookmark>(
                DefaultData.Schools.Select(school => new SchoolBookmark(school, false))
            );
            AllSchoolClassBookmarks = new HashSet<SchoolClassBookmark>(
                DefaultData.SchoolClasses.Select(schoolClass => new SchoolClassBookmark(schoolClass, false, false))
            );
            SchoolClassBookmarks = new ObservableCollection<SchoolClassBookmark>();
            NewSchoolClassBookmarks = new ObservableCollection<SchoolClassBookmark>();
        }

        public async Task Load()
        {
            if (LoadTask != null)
            {
                await LoadTask;
                return;
            }
            if (!Loaded)
            {
                Loaded = true;
                LoadTask = DoLoad();
                await LoadTask;
                LoadTask = null;
            }
        }

        async Task DoLoad()
        {
            SchoolVplan.Value = await Persistence.LoadVplan();
            NewSchoolClassBookmarks.AddRange(await Persistence.LoadNewSchoolClassBookmarks());
            SchoolBookmarks.UpdateOrInsertRange(await Persistence.LoadSchoolBookmarks());
            (await Persistence.LoadSchoolClassBookmarks()).ForEach(UpdateSchoolClassBookmark);
            UpdateBookmarkedVplan();
        }

        public async Task<VplanDiff> Refresh()
        {
            if (RefreshTask != null)
            {
                return await RefreshTask;
            }

            await Load();

            RefreshTask = DoRefresh();
            var diff = await RefreshTask;
            RefreshTask = null;
            return diff;
        }

        async Task<VplanDiff> DoRefresh()
        {
            LastRefreshFailed.Value = false;
            IsRefreshing.Value = true;

            var oldVplan = MyVplan.Value;
            var oldNewSchoolClasses = NewSchoolClassBookmarks.ToList();

            try
            {
                var newVplan = await Loader.Load();
                if (newVplan.Equals(SchoolVplan.Value))
                {
                    IsRefreshing.Value = false;
                    return new VplanDiff(false, new List<Change>(), new List<SchoolClassBookmark>());
                }
                SchoolVplan.Value = newVplan;
            } 
            catch (Exception e)
            {
                Debug.Print("Failed to refresh data: " + e);
                LastRefreshFailed.Value = true;
                IsRefreshing.Value = false;
                return null;
            }

            AddNewSchoolClasses(SchoolVplan.Value.Changes.Select(c => c.SchoolClass));
            UpdateBookmarkedVplan();

            await PersistAll();

            var gotUpdated = SchoolVplan.Value != null && (oldVplan == null || !oldVplan.LastUpdate.Equals(SchoolVplan.Value.LastUpdate));
            var oldBookmarkedChanges = new HashSet<Change>(oldVplan != null ? oldVplan.Changes : new ImmutableArray<Change>());
            var newBookmarkedChanges = new HashSet<Change>(MyVplan.Value != null ? MyVplan.Value.Changes : new ImmutableArray<Change>());
            var newBookmarks = newBookmarkedChanges.Except(oldBookmarkedChanges);
            var newNewClasses = new HashSet<SchoolClassBookmark>(NewSchoolClassBookmarks).Except(new HashSet<SchoolClassBookmark>(oldNewSchoolClasses));

            IsRefreshing.Value = false;
            return new VplanDiff(gotUpdated, newBookmarks.ToList(), newNewClasses.ToList());
        }

        public async Task ClearNewSchoolClassBookmarks()
        {
            NewSchoolClassBookmarks.Clear();
            await PersistNewSchoolClasses();
        }

        public async Task BookmarkSchool(string school, bool bookmark)
        {
            SchoolBookmarks.UpdateOrInsert(new SchoolBookmark(school, bookmark));

            AllSchoolClassBookmarks.Where( (arg) => arg.SchoolClass.School.Equals(school) )
                                   .ToList()
                                   .ForEach(schoolClassBookmark => {
                var newBookmark = new SchoolClassBookmark(schoolClassBookmark.SchoolClass,
                                                          schoolClassBookmark.Bookmarked,
                                                          bookmark);
                UpdateSchoolClassBookmark(newBookmark);
            });
            UpdateBookmarkedVplan();

            await PersistSchoolBookmarks();
            await PersistSchoolClassBookmarks();
        }   

        public async Task BookmarkSchoolClass(SchoolClass schoolClass, bool bookmark)
        {
            var i = IndexOfSchoolClassBookmark(schoolClass);
            Debug.Assert(i >= 0, string.Format("School class bookmark {0} schould exist", schoolClass));

            var oldBookmark = SchoolClassBookmarks[i];
            var newBookmark = new SchoolClassBookmark(schoolClass,
                                                      bookmark,
                                                      oldBookmark.SchoolBookmarked);
            UpdateSchoolClassBookmark(newBookmark);
            UpdateBookmarkedVplan();

            await PersistSchoolClassBookmarks();
        }

        void UpdateBookmarkedVplan()
        {
            if (SchoolVplan.Value == null)
            {
                return;
            }

            List<Change> bookmarkedChanges = new List<Change>();
            foreach (Change change in SchoolVplan.Value.Changes)
            {
                var bookmark = GetSchoolClassBookmark(change.SchoolClass);
                if (bookmark != null && bookmark.Bookmarked)
                {
                    bookmarkedChanges.Add(change);
                }
            }

            var newVplan = new Vplan(SchoolVplan.Value.LastUpdate, bookmarkedChanges);
            if (!newVplan.Equals(MyVplan.Value))
            {
                MyVplan.Value = newVplan;
            }
        }

        void AddNewSchoolClasses(IEnumerable<SchoolClass> schoolClasses)
        {
            foreach (SchoolClass schoolClass in schoolClasses)
            {
                var schoolBookmark = GetSchoolBookmark(schoolClass.School);
                Debug.Assert(schoolBookmark != null, string.Format("School class bookmark {0} schould exist", schoolClass));
                var newBookmark = new SchoolClassBookmark(schoolClass,
                                                         false,
                                                         schoolBookmark.Bookmarked);

                if (AllSchoolClassBookmarks.Contains(newBookmark))
                {
                    return;
                }

                if (schoolBookmark.Bookmarked)
                {
                    NewSchoolClassBookmarks.Add(newBookmark);
                }
                UpdateSchoolClassBookmark(newBookmark);
            }
        }

        void UpdateSchoolClassBookmark(SchoolClassBookmark newBookmark)
        {
            // Replace bookmark
            AllSchoolClassBookmarks.Remove(newBookmark);
            AllSchoolClassBookmarks.Add(newBookmark);

            var newBookmarkIndex = NewSchoolClassBookmarks.IndexOf(newBookmark);
            if (newBookmarkIndex >= 0)
            {
                NewSchoolClassBookmarks[newBookmarkIndex] = newBookmark;
            }

            if (newBookmark.SchoolBookmarked)
            {
                SchoolClassBookmarks.UpdateOrInsert(newBookmark);
            }
            else
            {
                SchoolClassBookmarks.Remove(newBookmark);
            }
        }

        int IndexOfSchoolClassBookmark(SchoolClass schoolClass)
        {
            var dummyBookmark = new SchoolClassBookmark(schoolClass, false, false);
            return SchoolClassBookmarks.IndexOf(dummyBookmark);
        }

        SchoolClassBookmark GetSchoolClassBookmark(SchoolClass schoolClass)
        {
            var i = IndexOfSchoolClassBookmark(schoolClass);
            if (i < 0)
            {
                return null;
            }
            return SchoolClassBookmarks[i];
        }

        int IndexOfSchoolBookmark(string school)
        {
            var dummyBookmark = new SchoolBookmark(school, false);
            return SchoolBookmarks.IndexOf(dummyBookmark);
        }

        SchoolBookmark GetSchoolBookmark(string school)
        {
            var i = IndexOfSchoolBookmark(school);
            if (i < 0)
            {
                return null;
            }
            return SchoolBookmarks[i];
        }

        async Task PersistSchoolBookmarks()
        {
            await Persistence.PersistSchoolBookmarks(SchoolBookmarks.ToList());
        }

        async Task PersistSchoolClassBookmarks()
        {
            await Persistence.PersistSchoolClassBookmarks(AllSchoolClassBookmarks.ToList());
        }

        async Task PersistNewSchoolClasses()
        {
            await Persistence.PersistNewSchoolClassBookmarks(NewSchoolClassBookmarks.ToList());
        }

        async Task PersistAll()
        {
            await Persistence.PersistVplan(SchoolVplan.Value);
            await PersistNewSchoolClasses();
            await PersistSchoolBookmarks();
            await PersistSchoolClassBookmarks();
        }
    }
}
