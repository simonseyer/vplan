using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace FLSVertretungsplan
{
    public class JsonVplanPersistence: IVplanPersistence
    {
        static string NewClassesFileName = "newSchoolClassBookmarks.json";
        static string SchoolBookmarksFileName = "schoolBookmarks.json";
        static string SchoolClassBookmarksFileName = "schoolClassBookmarks.json";
        static string TeacherBookmarksFileName = "teacherBookmarks.json";
        static string VplanFileName = "vplan.json";

        // Prevent loading and persisting the json files at the same time by
        // locking access to the files. Sharing violation could be raised otherwise.
        SemaphoreSlim NewSchoolClassBookmarksSempahore = new SemaphoreSlim(1, 1);
        SemaphoreSlim SchoolBookmarksSempahore = new SemaphoreSlim(1, 1);
        SemaphoreSlim SchoolClassBookmarksSempahore = new SemaphoreSlim(1, 1);
        SemaphoreSlim TeacherBookmarksSempahore = new SemaphoreSlim(1, 1);
        SemaphoreSlim VplanSempahore = new SemaphoreSlim(1, 1);

        public async Task<List<SchoolClassBookmark>> LoadNewSchoolClassBookmarks()
        {
            return await Load(NewClassesFileName, NewSchoolClassBookmarksSempahore, new List<SchoolClassBookmark>());
        }

        public async Task<List<SchoolBookmark>> LoadSchoolBookmarks()
        {
            return await Load(SchoolBookmarksFileName, SchoolBookmarksSempahore, new List<SchoolBookmark>());
        }

        public async Task<List<SchoolClassBookmark>> LoadSchoolClassBookmarks()
        {
            return await Load(SchoolClassBookmarksFileName, SchoolClassBookmarksSempahore, new List<SchoolClassBookmark>());
        }

        public async Task<List<TeacherBookmark>> LoadTeacherBookmarks()
        {
            return await Load(TeacherBookmarksFileName, TeacherBookmarksSempahore, new List<TeacherBookmark>());
        }

        public async Task<Vplan> LoadVplan()
        {
            return await Load<Vplan>(VplanFileName, VplanSempahore, null);
        }


        public async Task PersistNewSchoolClassBookmarks(List<SchoolClassBookmark> newSchoolClasses)
        {
            await Persist(newSchoolClasses, NewClassesFileName, NewSchoolClassBookmarksSempahore);
        }

        public async Task PersistSchoolBookmarks(List<SchoolBookmark> schoolBookmarks)
        {
            await Persist(schoolBookmarks, SchoolBookmarksFileName, SchoolBookmarksSempahore);
        }

        public async Task PersistSchoolClassBookmarks(List<SchoolClassBookmark> schoolClassBookmarks)
        {
            await Persist(schoolClassBookmarks, SchoolClassBookmarksFileName, SchoolClassBookmarksSempahore);
        }

        public async Task PersistTeacherBookmarks(List<TeacherBookmark> teacherBookmarks)
        {
            await Persist(teacherBookmarks, TeacherBookmarksFileName, TeacherBookmarksSempahore);
        }

        public async Task PersistVplan(Vplan vplan)
        {
            await Persist(vplan, VplanFileName, VplanSempahore);
        }


        async Task Persist(object theObject, string fileName, SemaphoreSlim semaphore)
        {
            await semaphore.WaitAsync();
            try
            {
                var json = await Task.Run(() => JsonConvert.SerializeObject(theObject));
                var filePath = CreatePersistentPath(fileName);
                using (var file = File.Open(filePath, FileMode.Create, FileAccess.Write))
                using (var strm = new StreamWriter(file))
                {
                    await strm.WriteAsync(json);
                }
            } catch {
                throw;
            } finally {
                semaphore.Release();
            }
        }

        async Task<T> Load<T>(string fileName, SemaphoreSlim semaphore, T defaultValue)
        {
            await semaphore.WaitAsync();
            try
            {
                var filePath = CreatePersistentPath(fileName);
                using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read))
                using (var strm = new StreamReader(file))
                {
                    var json = await strm.ReadToEndAsync();
                    return await Task.Run(() => JsonConvert.DeserializeObject<T>(json));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return defaultValue;
            }
            finally
            {
                semaphore.Release();
            }
        }

        string CreatePersistentPath(string fileName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(path, fileName);
        }
    }
}
