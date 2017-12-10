using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;

namespace FLSVertretungsplan
{
    public class CloudVplanDataStore: IVplanDataStore
    {

        HttpClient client;
        Vplan vplan;
        Property<Vplan> bookmarkedVplan;
        ObservableCollection<SchoolBookmark> bookmarkedSchools;
        HashSet<SchoolClassBookmark> schoolClasses;
        ObservableCollection<SchoolClassBookmark> bookmarkedClasses;

        public CloudVplanDataStore()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"{App.BackendUrl}/");

            vplan = new Vplan
            {
                LastUpdate = DateTime.Now,
                Changes = new List<Change>()
            };
            bookmarkedVplan = new Property<Vplan>()
            {
                Value = new Vplan
                {
                    LastUpdate = DateTime.Now,
                    Changes = new List<Change>()
                }
            };
            bookmarkedSchools = new ObservableCollection<SchoolBookmark>() 
            {
                new SchoolBookmark("BG", false),
                new SchoolBookmark("BS", false),
                new SchoolBookmark("BFS", false),
                new SchoolBookmark("HBFS", false)
            };
            schoolClasses = new HashSet<SchoolClassBookmark>();
            bookmarkedClasses = new ObservableCollection<SchoolClassBookmark>();
        }

        public async Task<Vplan> GetVplanAsync(bool forceRefresh = false)
        {
            if (forceRefresh)
            {
                var xml = await client.GetStringAsync($"raw/vplan?version=1.2.4");
                vplan = await Task.Run(() => VplanParser.Parse(xml));
                AddClasses(vplan);
                UpdateBookmarkedVplan();
            }
            return vplan;
        }

        public Property<Vplan> GetBookmarkedVplan() 
        {
            return bookmarkedVplan;
        }

        public ObservableCollection<SchoolBookmark> GetBookmarkedSchools()
        {
            return bookmarkedSchools;
        }

        public ObservableCollection<SchoolClassBookmark> GetBookmarkedClasses()
        {
            return bookmarkedClasses;
        }

        private void UpdateBookmarkedVplan()
        {
            List<Change> bookmarkedChanges = new List<Change>();
            foreach (Change change in vplan.Changes)
            {
                var bookmark = GetSchoolClassBookmark(change.SchoolClass);
                if (bookmark != null && bookmark.Bookmarked)
                {
                    bookmarkedChanges.Add(change);
                }
            }

            bookmarkedVplan.Value = new Vplan
            {
                LastUpdate = vplan.LastUpdate,
                Changes = bookmarkedChanges
            };
        }

        private void AddClasses(Vplan vplan)
        {
            foreach (Change change in vplan.Changes)
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

            if (schoolClasses.Contains(newBookmark))
            {
                return;
            }

            UpdateBookmarkedSchoolClass(newBookmark);
        }

        private void UpdateBookmarkedSchoolClass(SchoolClassBookmark newBookmark)
        {
            schoolClasses.Remove(newBookmark);
            schoolClasses.Add(newBookmark);
            if (newBookmark.SchoolBookmarked)
            {
                for (var i = 0; i < bookmarkedClasses.Count; i++)
                {
                    var bookmark = bookmarkedClasses[i];
                    var comparison = bookmark.SchoolClass.CompareTo(newBookmark.SchoolClass);
                    if (comparison == 0)
                    {
                        bookmarkedClasses[i] = newBookmark;
                        return;
                    }
                    else if (comparison > 0)
                    {
                        bookmarkedClasses.Insert(i, newBookmark);
                        return;
                    }
                }
                bookmarkedClasses.Add(newBookmark);
            }
            else
            {
                bookmarkedClasses.Remove(newBookmark);
            }
        }

        public void BookmarkSchool(string school, bool bookmark)
        {
            var i = IndexOfSchoolBookmark(school);
            Debug.Assert(i >= 0, string.Format("School bookmark {0} schould exist", school));
            bookmarkedSchools[i] = new SchoolBookmark(school, bookmark);

            foreach (var schoolClassBookmark in new HashSet<SchoolClassBookmark>(schoolClasses))
            {
                if (schoolClassBookmark.SchoolClass.School.Equals(school))
                {
                    var newBookmark = new SchoolClassBookmark(schoolClassBookmark.SchoolClass,
                                                              schoolClassBookmark.Bookmarked,
                                                              bookmark);
                    UpdateBookmarkedSchoolClass(newBookmark);
                }
            }
            UpdateBookmarkedVplan();
        }

        public void BookmarkClass(SchoolClass schoolClass, bool bookmark)
        {
            var i = IndexOfSchoolClassBookmark(schoolClass);
            Debug.Assert(i >= 0, string.Format("School class bookmark {0} schould exist", schoolClass));

            var oldBookmark = bookmarkedClasses[i];
            var newBookmark = new SchoolClassBookmark(schoolClass, 
                                                      bookmark, 
                                                      oldBookmark.SchoolBookmarked);
            UpdateBookmarkedSchoolClass(newBookmark);
            UpdateBookmarkedVplan();
        }

        private int IndexOfSchoolClassBookmark(SchoolClass schoolClass)
        {
            var dummyBookmark = new SchoolClassBookmark(schoolClass, false, false);
            return bookmarkedClasses.IndexOf(dummyBookmark);
        }

        private SchoolClassBookmark GetSchoolClassBookmark(SchoolClass schoolClass)
        {
            var i = IndexOfSchoolClassBookmark(schoolClass);
            if (i < 0)
            {
                return null;
            }
            return bookmarkedClasses[i];
        }

        private int IndexOfSchoolBookmark(string school)
        {
            var dummyBookmark = new SchoolBookmark(school, false);
            return bookmarkedSchools.IndexOf(dummyBookmark);
        }

        private SchoolBookmark GetSchoolBookmark(string school)
        {
            var i = IndexOfSchoolBookmark(school);
            if (i < 0)
            {
                return null;
            }
            return bookmarkedSchools[i];
        }

    }

}
