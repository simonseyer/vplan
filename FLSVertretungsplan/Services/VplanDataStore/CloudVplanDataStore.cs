﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace FLSVertretungsplan
{
    public class CloudVplanDataStore: IVplanDataStore
    {

        IVplanLoader Loader => ServiceLocator.Instance.Get<IVplanLoader>();
        IVplanPersistence Persistence => ServiceLocator.Instance.Get<IVplanPersistence>();

        // Transient state
        public Property<bool> IsRefreshing { get; }
        public Property<bool> LastRefreshFailed { get; }

        // State
        public Property<Vplan> Vplan { get; }
        public HashSet<SchoolClassBookmark> AllSchoolClassBookmarks { get; }
        public ObservableCollection<SchoolBookmark> SchoolBookmarks { get; }
        public ObservableCollection<SchoolClassBookmark> NewSchoolClassBookmarks { get; }

        // Derived state
        public Property<Vplan> BookmarkedVplan { get; }
        // Contains only SchoolClassBookmarks of the bookmarked schools
        public ObservableCollection<SchoolClassBookmark> SchoolClassBookmarks { get; }

        Task LoadTask;
        Task<VplanDiff> RefreshTask;
        bool Loaded;

        public CloudVplanDataStore()
        {
            
            IsRefreshing = new Property<bool>();
            LastRefreshFailed = new Property<bool>();

            Vplan = new Property<Vplan>
            {
                Value = null
            };
            BookmarkedVplan = new Property<Vplan>()
            {
                Value = null
            };
            SchoolBookmarks = new ObservableCollection<SchoolBookmark>() 
            {
                new SchoolBookmark("BG", false),
                new SchoolBookmark("BS", false),
                new SchoolBookmark("BFS", false),
                new SchoolBookmark("HBFS", false)
            };
            AllSchoolClassBookmarks = new HashSet<SchoolClassBookmark>
            {
                new SchoolClassBookmark(new SchoolClass("1181", "BFS"), false, false),
                new SchoolClassBookmark(new SchoolClass("11 REFILAS", "BG"), false, false),
                new SchoolClassBookmark(new SchoolClass("11/1 W", "BG"), false, false),
                new SchoolClassBookmark(new SchoolClass("11/2 W", "BG"), false, false),
                new SchoolClassBookmark(new SchoolClass("11/3 W", "BG"), false, false),
                new SchoolClassBookmark(new SchoolClass("11/6 DV", "BG"), false, false),
                new SchoolClassBookmark(new SchoolClass("12", "BG"), false, false),
                new SchoolClassBookmark(new SchoolClass("13", "BG"), false, false),
                new SchoolClassBookmark(new SchoolClass("1051", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1061", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1071", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1151", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1222", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1251", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("InteA 3", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1191", "HBFS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1291", "HBFS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1082", "BFS"), false, false),
                new SchoolClassBookmark(new SchoolClass("11/8 CT", "BG"), false, false),
                new SchoolClassBookmark(new SchoolClass("1031", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1035", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1037", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1162", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1225", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1193", "HBFS"), false, false),
                new SchoolClassBookmark(new SchoolClass("11/7 DV", "BG"), false, false),
                new SchoolClassBookmark(new SchoolClass("1021", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1038", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1137", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1141", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1163", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1171", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1245", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("1271", "BS"), false, false),
                new SchoolClassBookmark(new SchoolClass("11/5 W/Bili", "BG"), false, false)
            };
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
            if (Loaded)
            {
                return;
            }
            Loaded = true;

            LoadTask = DoLoad();
            await LoadTask;
            LoadTask = null;
        }

        async Task DoLoad()
        {
            Vplan.Value = await Persistence.LoadVplan();

            var newSchoolClassBookmarks = await Persistence.LoadNewSchoolClassBookmarks();
            foreach (var schoolClassBookmark in newSchoolClassBookmarks)
            {
                NewSchoolClassBookmarks.Add(schoolClassBookmark);
            }

            var schoolBookmarks = await Persistence.LoadSchoolBookmarks();
            foreach (var bookmark in schoolBookmarks)
            {
                var i = SchoolBookmarks.IndexOf(bookmark);
                if (i >= 0)
                {
                    SchoolBookmarks[i] = bookmark;
                }
                else
                {
                    SchoolBookmarks.Add(bookmark);
                }
            }

            var schoolClassBookmarks = await Persistence.LoadSchoolClassBookmarks();
            foreach (var bookmark in schoolClassBookmarks)
            {
                Debug.Print(string.Format("new SchoolClassBookmark(new SchoolClass(\"{0}\", \"{1}\"), false, false),", bookmark.SchoolClass.Name, bookmark.SchoolClass.School));
                UpdateSchoolClassBookmark(bookmark);
            }

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

            var oldVplan = BookmarkedVplan.Value;
            var oldNewSchoolClasses = NewSchoolClassBookmarks.ToList();

            try
            {
                var newVplan = await Loader.Load();
                if (newVplan.Equals(Vplan.Value))
                {
                    IsRefreshing.Value = false;
                    return new VplanDiff(false, new List<Change>(), new List<SchoolClassBookmark>());
                }
                Vplan.Value = newVplan;
            } 
            catch (Exception e)
            {
                Debug.Print("Failed to refresh data: " + e);
                LastRefreshFailed.Value = true;
                IsRefreshing.Value = false;
                return null;
            }

            UpdateSchoolClasses();
            UpdateBookmarkedVplan();

            await PersistAll();

            var gotUpdated = Vplan.Value != null && (oldVplan == null || !oldVplan.LastUpdate.Equals(Vplan.Value.LastUpdate));
            var oldBookmarkedChanges = oldVplan?.Changes.ToHashSet() ?? new HashSet<Change>();
            var newBookmarkedChanges = BookmarkedVplan.Value?.Changes.ToHashSet() ?? new HashSet<Change>();
            var newBookmarks = newBookmarkedChanges.Except(oldBookmarkedChanges);
            var newNewClasses = NewSchoolClassBookmarks.ToHashSet().Except(oldNewSchoolClasses.ToHashSet());

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
            var i = IndexOfSchoolBookmark(school);
            Debug.Assert(i >= 0, string.Format("School bookmark {0} schould exist", school));
            SchoolBookmarks[i] = new SchoolBookmark(school, bookmark);

            foreach (var schoolClassBookmark in new HashSet<SchoolClassBookmark>(AllSchoolClassBookmarks))
            {
                if (schoolClassBookmark.SchoolClass.School.Equals(school))
                {
                    var newBookmark = new SchoolClassBookmark(schoolClassBookmark.SchoolClass,
                                                              schoolClassBookmark.Bookmarked,
                                                              bookmark);
                    UpdateSchoolClassBookmark(newBookmark);
                }
            }
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
            if (Vplan.Value == null)
            {
                return;
            }

            List<Change> bookmarkedChanges = new List<Change>();
            foreach (Change change in Vplan.Value.Changes)
            {
                var bookmark = GetSchoolClassBookmark(change.SchoolClass);
                if (bookmark != null && bookmark.Bookmarked)
                {
                    bookmarkedChanges.Add(change);
                }
            }

            var newVplan = new Vplan(Vplan.Value.LastUpdate, bookmarkedChanges);
            if (!newVplan.Equals(BookmarkedVplan.Value))
            {
                BookmarkedVplan.Value = newVplan;
            }
        }

        void UpdateSchoolClasses()
        {
            foreach (Change change in Vplan.Value.Changes)
            {
                AddClass(change.SchoolClass);
            }
        }

        void AddClass(SchoolClass schoolClass)
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
                for (var i = 0; i < SchoolClassBookmarks.Count; i++)
                {
                    var bookmark = SchoolClassBookmarks[i];
                    var comparison = bookmark.SchoolClass.CompareTo(newBookmark.SchoolClass);
                    if (comparison == 0)
                    {
                        SchoolClassBookmarks[i] = newBookmark;
                        return;
                    }
                    else if (comparison > 0)
                    {
                        SchoolClassBookmarks.Insert(i, newBookmark);
                        return;
                    }
                }
                SchoolClassBookmarks.Add(newBookmark);
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
            await Persistence.PersistVplan(Vplan.Value);
            await PersistNewSchoolClasses();
            await PersistSchoolBookmarks();
            await PersistSchoolClassBookmarks();
        }
    }
}