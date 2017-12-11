using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;

namespace FLSVertretungsplan
{
    public class CloudVplanDataStore: IVplanDataStore
    {

        HttpClient client;

        // Transient state
        Property<bool> IsRefreshing;

        // State
        Property<Vplan> Vplan;
        HashSet<SchoolClassBookmark> AllSchoolClassBookmarks;
        ObservableCollection<SchoolBookmark> SchoolBookmarks;

        // Derived state
        Property<Vplan> BookmarkedVplan;
        // Contains only SchoolClassBookmarks of the bookmarked schools
        ObservableCollection<SchoolClassBookmark> SchoolClassBookmarks;

        public CloudVplanDataStore()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri($"{App.BackendUrl}/");

            IsRefreshing = new Property<bool>();

            Vplan = new Property<Vplan> {
                Value = new Vplan
                {
                    LastUpdate = DateTime.Now,
                    Changes = new List<Change>()
                }
            };
            BookmarkedVplan = new Property<Vplan>()
            {
                Value = new Vplan
                {
                    LastUpdate = DateTime.Now,
                    Changes = new List<Change>()
                }
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

            _ = LoadPersistedData();
        }

        public async Task Refresh()
        {
            if (IsRefreshing.Value)
            {
                return;
            }
            IsRefreshing.Value = true;
            
            var xml = await client.GetStringAsync($"raw/vplan?version=1.2.4");
            Vplan.Value = await Task.Run(() => VplanParser.Parse(xml));

            UpdateSchoolClasses();
            UpdateBookmarkedVplan();

            _ = PersistAll();

            IsRefreshing.Value = false;
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

            _ = PersistSchoolBookmarks();
            _ = PersistSchoolClassBookmarks();
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

            _ = PersistSchoolClassBookmarks();
        }

        public Property<bool> GetIsRefreshing()
        {
            return IsRefreshing;
        }

        public Property<Vplan> GetVplan()
        {
            return Vplan;
        }

        public Property<Vplan> GetBookmarkedVplan() 
        {
            return BookmarkedVplan;
        }

        public ObservableCollection<SchoolBookmark> GetSchoolBookmarks()
        {
            return SchoolBookmarks;
        }

        public ObservableCollection<SchoolClassBookmark> GetSchoolClassBookmarks()
        {
            return SchoolClassBookmarks;
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

            BookmarkedVplan.Value = new Vplan
            {
                LastUpdate = Vplan.Value.LastUpdate,
                Changes = bookmarkedChanges
            };
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
            await PersistVplan();
            await PersistSchoolBookmarks();
            await PersistSchoolClassBookmarks();
        }

        private async Task PersistVplan()
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(Vplan.Value));
            await PersistJson(json, "vplan.json");
            //Vplan.Value = JsonConvert.DeserializeObject<Vplan>(json);
        }

        private async Task PersistSchoolBookmarks()
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(SchoolBookmarks));
            await PersistJson(json, "schoolBookmarks.json");
        }

        private async Task PersistSchoolClassBookmarks()
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(SchoolClassBookmarks));
            await PersistJson(json, "schoolClassBookmarks.json");
        }

        private async Task PersistJson(string json, string fileName)
        {
            var filePath = CreatePersistentPath(fileName);
            using (var file = File.Open(filePath, FileMode.Create, FileAccess.Write))
            using (var strm = new StreamWriter(file))
            {
                await strm.WriteAsync(json);
            }
        }

        private async Task<string> LoadJson(string fileName)
        {
            var filePath = CreatePersistentPath(fileName);
            using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var strm = new StreamReader(file))
            {
                return await strm.ReadToEndAsync();
            }
        }

        private string CreatePersistentPath(string fileName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(path, fileName);
        }

        private async Task LoadPersistedData()
        {
            var json = await LoadJson("vplan.json");
            Vplan.Value = await Task.Run(() => JsonConvert.DeserializeObject<Vplan>(json));

            json = await LoadJson("schoolBookmarks.json");
            var schoolBookmarks = await Task.Run(() => JsonConvert.DeserializeObject<Collection<SchoolBookmark>>(json));
            foreach (var bookmark in schoolBookmarks)
            {
                SchoolBookmarks.Add(bookmark);
            }

            json = await LoadJson("schoolClassBookmarks.json");
            var schoolClassBookmarks = await Task.Run(() => JsonConvert.DeserializeObject<Collection<SchoolClassBookmark>>(json));
            foreach (var bookmark in schoolClassBookmarks)
            {
                UpdateSchoolClassBookmark(bookmark);
            }

            UpdateBookmarkedVplan();
        }
    }
}
