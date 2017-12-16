using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;

namespace FLSVertretungsplan
{
    public class CloudVplanDataStore: IVplanDataStore
    {

        private IVplanLoader Loader => ServiceLocator.Instance.Get<IVplanLoader>();
        private IVplanPersistence Persistence => ServiceLocator.Instance.Get<IVplanPersistence>();

        // Transient state
        public Property<bool> IsRefreshing { get; }

        // State
        public Property<Vplan> Vplan { get; }
        public HashSet<SchoolClassBookmark> AllSchoolClassBookmarks { get; }
        public ObservableCollection<SchoolBookmark> SchoolBookmarks { get; }
        public ObservableCollection<SchoolClass> NewSchoolClasses { get; }

        // Derived state
        public Property<Vplan> BookmarkedVplan { get; }
        // Contains only SchoolClassBookmarks of the bookmarked schools
        public ObservableCollection<SchoolClassBookmark> SchoolClassBookmarks { get; }

        public CloudVplanDataStore()
        {
            
            IsRefreshing = new Property<bool>();

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
            AllSchoolClassBookmarks = new HashSet<SchoolClassBookmark>();
            SchoolClassBookmarks = new ObservableCollection<SchoolClassBookmark>();
            NewSchoolClasses = new ObservableCollection<SchoolClass>();
        }

        public async Task Load()
        {
            Vplan.Value = await Persistence.LoadVplan();

            var newSchoolClasses = await Persistence.LoadNewSchoolClasses();
            foreach (var schoolClass in newSchoolClasses)
            {
                NewSchoolClasses.Add(schoolClass);
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
                UpdateSchoolClassBookmark(bookmark);
            }

            UpdateBookmarkedVplan();
        }

        public async Task Refresh()
        {
            if (IsRefreshing.Value)
            {
                return;
            }
            IsRefreshing.Value = true;

            try
            {

                Vplan.Value = await Loader.Load();
            } 
            catch (Exception e) 
            {
                Debug.Print("Failed to refresh data: " + e);
                IsRefreshing.Value = false;
                throw e;
            }

            UpdateSchoolClasses();
            UpdateBookmarkedVplan();

            await PersistAll();

            IsRefreshing.Value = false;
        }

        public void ClearNewSchoolClasses()
        {
            NewSchoolClasses.Clear();
            _ = Persistence.PersistNewSchoolClasses(NewSchoolClasses.ToList());
        }

        public void BookmarkSchool(string school, bool bookmark)
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

            Persistence.PersistSchoolBookmarks(SchoolBookmarks.ToList());
            Persistence.PersistSchoolClassBookmarks(SchoolClassBookmarks.ToList());
        }

        public void BookmarkSchoolClass(SchoolClass schoolClass, bool bookmark)
        {
            var i = IndexOfSchoolClassBookmark(schoolClass);
            Debug.Assert(i >= 0, string.Format("School class bookmark {0} schould exist", schoolClass));

            var oldBookmark = SchoolClassBookmarks[i];
            var newBookmark = new SchoolClassBookmark(schoolClass,
                                                      bookmark,
                                                      oldBookmark.SchoolBookmarked);
            UpdateSchoolClassBookmark(newBookmark);
            UpdateBookmarkedVplan();

            Persistence.PersistSchoolClassBookmarks(SchoolClassBookmarks.ToList());
        }

        private void UpdateBookmarkedVplan()
        {
            List<Change> bookmarkedChanges = new List<Change>();
            foreach (Change change in Vplan.Value.Changes)
            {
                var bookmark = GetSchoolClassBookmark(change.SchoolClass);
                if (bookmark != null && bookmark.Bookmarked)
                {
                    bookmarkedChanges.Add(change);
                }
            }

            BookmarkedVplan.Value = new Vplan(Vplan.Value.LastUpdate, bookmarkedChanges);
        }

        private void UpdateSchoolClasses()
        {
            foreach (Change change in Vplan.Value.Changes)
            {
                AddClass(change.SchoolClass);
            }
        }

        private void AddClass(SchoolClass schoolClass)
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

            NewSchoolClasses.Add(schoolClass);
            UpdateSchoolClassBookmark(newBookmark);
        }

        private void UpdateSchoolClassBookmark(SchoolClassBookmark newBookmark)
        {
            // Replace bookmark
            AllSchoolClassBookmarks.Remove(newBookmark);
            AllSchoolClassBookmarks.Add(newBookmark);

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

        private int IndexOfSchoolClassBookmark(SchoolClass schoolClass)
        {
            var dummyBookmark = new SchoolClassBookmark(schoolClass, false, false);
            return SchoolClassBookmarks.IndexOf(dummyBookmark);
        }

        private SchoolClassBookmark GetSchoolClassBookmark(SchoolClass schoolClass)
        {
            var i = IndexOfSchoolClassBookmark(schoolClass);
            if (i < 0)
            {
                return null;
            }
            return SchoolClassBookmarks[i];
        }

        private int IndexOfSchoolBookmark(string school)
        {
            var dummyBookmark = new SchoolBookmark(school, false);
            return SchoolBookmarks.IndexOf(dummyBookmark);
        }

        private SchoolBookmark GetSchoolBookmark(string school)
        {
            var i = IndexOfSchoolBookmark(school);
            if (i < 0)
            {
                return null;
            }
            return SchoolBookmarks[i];
        }

        private async Task PersistAll()
        {
            await Persistence.PersistVplan(Vplan.Value);
            await Persistence.PersistNewSchoolClasses(NewSchoolClasses.ToList());
            await Persistence.PersistSchoolBookmarks(SchoolBookmarks.ToList());
            await Persistence.PersistSchoolClassBookmarks(AllSchoolClassBookmarks.ToList());
        }

    }
}
