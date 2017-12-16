using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace FLSVertretungsplan
{
    public class JsonVplanPersistence: IVplanPersistence
    {
        static string newClassesFileName = "newSchoolClasses.json";
        static string schoolBookmarksFileName = "schoolBookmarks.json";
        static string schoolClassBookmarksFileName = "schoolClassBookmarks.json";
        static string vplanFileName = "vplan.json";

        public async Task<List<SchoolClass>> LoadNewSchoolClasses()
        {
            try
            {
                return await Load<List<SchoolClass>>(newClassesFileName) ?? new List<SchoolClass>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new List<SchoolClass>();
            }
        }

        public async Task<List<SchoolBookmark>> LoadSchoolBookmarks()
        {
            try
            {
                return await Load<List<SchoolBookmark>>(schoolBookmarksFileName) ?? new List<SchoolBookmark>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new List<SchoolBookmark>();
            }
        }

        public async Task<List<SchoolClassBookmark>> LoadSchoolClassBookmarks()
        {
            try
            {
                return await Load<List<SchoolClassBookmark>>(schoolClassBookmarksFileName) ?? new List<SchoolClassBookmark>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return new List<SchoolClassBookmark>();
            }
        }

        public async Task<Vplan> LoadVplan()
        {
            try
            {
                return await Load<Vplan>(vplanFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task PersistNewSchoolClasses(List<SchoolClass> newSchoolClasses)
        {
            await Persist(newSchoolClasses, newClassesFileName);
        }

        public async Task PersistSchoolBookmarks(List<SchoolBookmark> schoolBookmarks)
        {
            await Persist(schoolBookmarks, schoolBookmarksFileName);
        }

        public async Task PersistSchoolClassBookmarks(List<SchoolClassBookmark> schoolClassBookmarks)
        {
            await Persist(schoolClassBookmarks, schoolClassBookmarksFileName);
        }

        public async Task PersistVplan(Vplan vplan)
        {
            await Persist(vplan, vplanFileName);
        }

        private async Task Persist(object theObject, string fileName)
        {
            var json = await Task.Run(() => JsonConvert.SerializeObject(theObject));
            var filePath = CreatePersistentPath(fileName);
            using (var file = File.Open(filePath, FileMode.Create, FileAccess.Write))
            using (var strm = new StreamWriter(file))
            {
                await strm.WriteAsync(json);
            }
        }

        private async Task<T> Load<T>(string fileName)
        {
            var filePath = CreatePersistentPath(fileName);
            using (var file = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var strm = new StreamReader(file))
            {
                var json = await strm.ReadToEndAsync();
                return await Task.Run(() => JsonConvert.DeserializeObject<T>(json));
            }
        }

        private string CreatePersistentPath(string fileName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(path, fileName);
        }
    }
}
