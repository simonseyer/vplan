using NUnit.Framework;
using System;
using FLSVertretungsplan;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace FLSVertretungsplanTests
{
    [TestFixture]
    public class CloudVplanDataStoreTest
    {
        [SetUp]
        public void Init()
        {
            ServiceLocator.Instance.Register<IDefaultData, MockDefaultData>();
            ServiceLocator.Instance.Register<IVplanLoader, MockVplanLoader>();
            ServiceLocator.Instance.Register<IVplanPersistence, JsonVplanPersistence>();
        }

        [Test]
        public void TestInitialState()  
        {
            IVplanDataStore dataStore = new CloudVplanDataStore();

            Assert.That(dataStore.SchoolBookmarks, Has.Count.EqualTo(2));
            Assert.That(dataStore.SchoolClassBookmarks, Has.Count.EqualTo(0));
        }

        [Test]
        public async Task TestSchoolBookmarksUpdated()
        {
            IVplanDataStore dataStore = new CloudVplanDataStore();

            await dataStore.BookmarkSchool("SCHOOL_A", true);
            Assert.That(dataStore.GetSchoolBookmark("SCHOOL_A").Bookmarked, Is.True);

            await dataStore.BookmarkSchool("SCHOOL_A", false);
            Assert.That(dataStore.GetSchoolBookmark("SCHOOL_A").Bookmarked, Is.False);
        }

        [Test]
        public async Task TestSchoolBookmarkedSchoolClassBookmarksUpdated()
        {
            IVplanDataStore dataStore = new CloudVplanDataStore();

            await dataStore.BookmarkSchool("SCHOOL_A", true);
            Assert.That(dataStore.SchoolClassBookmarks, Has.Count.EqualTo(2));

            await dataStore.BookmarkSchool("SCHOOL_A", false);
            Assert.That(dataStore.SchoolClassBookmarks, Has.Count.EqualTo(0));
        }

        [Test] 
        public async Task TestSchoolClassBookmarksUpdated()
        {
            IVplanDataStore dataStore = new CloudVplanDataStore();
            await dataStore.BookmarkSchool("SCHOOL_A", true);

            await dataStore.BookmarkSchoolClass(dataStore.GetSchoolClassBookmark("CLASS_A").SchoolClass, true);
            Assert.That(dataStore.GetSchoolClassBookmark("CLASS_A").Bookmarked, Is.True);

            await dataStore.BookmarkSchoolClass(dataStore.GetSchoolClassBookmark("CLASS_A").SchoolClass, false);
            Assert.That(dataStore.GetSchoolClassBookmark("CLASS_A").Bookmarked, Is.False);
        }

    }

    static class DataStore
    {
        public static SchoolBookmark GetSchoolBookmark(this IVplanDataStore dataStore, string name)
        {
            return dataStore.SchoolBookmarks.First(bookmark => bookmark.School == name);
        }

        public static SchoolClassBookmark GetSchoolClassBookmark(this IVplanDataStore dataStore, string name)
        {
            return dataStore.SchoolClassBookmarks.First(bookmark => bookmark.SchoolClass.Name == name);
        }
    }

    class MockDefaultData : IDefaultData
    {
        public Collection<string> Schools => new Collection<string>
        {
            "SCHOOL_A",
            "SCHOOL_B"
        };

        public Collection<SchoolClass> SchoolClasses => new Collection<SchoolClass>
        {
            new SchoolClass("SCHOOL_A", "CLASS_A"),
            new SchoolClass("SCHOOL_A", "CLASS_C"),
            new SchoolClass("SCHOOL_B", "CLASS_B")
        };
    }


    class MockVplanLoader : IVplanLoader
    {
        public Task<Vplan> Load()
        {
            var vplan = new Vplan(DateTime.Now, new List<Change>() {
                new Change(
                    new SchoolClass("SCHOOL_A", "CLASS_A"),
                    DateTime.Now,
                    "1",
                    new Lesson(new Subject("DEUT", "Deutsch"), new Teacher("Martin", "Müller", "MM"), "A123"),
                    new Lesson(new Subject("MAT", "Mathe"), new Teacher("Julia", "Schmidt", "JS"), "B456"),
                    "",
                    ""
                ),
            });
            return Task.FromResult(vplan);
        }
    }

    class MockVplanPersistence : IVplanPersistence
    {
        public Task<Vplan> LoadVplan() => Task.FromResult(new Vplan(DateTime.Now, new List<Change>()));

        public Task<List<SchoolClassBookmark>> LoadNewSchoolClassBookmarks() => Task.FromResult(new List<SchoolClassBookmark>());

        public Task<List<SchoolBookmark>> LoadSchoolBookmarks() => Task.FromResult(new List<SchoolBookmark>());

        public Task<List<SchoolClassBookmark>> LoadSchoolClassBookmarks() => Task.FromResult(new List<SchoolClassBookmark>());


        public Task PersistVplan(Vplan vplan) => Task.CompletedTask;

        public Task PersistNewSchoolClassBookmarks(List<SchoolClassBookmark> newSchoolClassBookmarks) => Task.CompletedTask;

        public Task PersistSchoolBookmarks(List<SchoolBookmark> schoolBookmarks) => Task.CompletedTask;

        public Task PersistSchoolClassBookmarks(List<SchoolClassBookmark> schoolClassBookmarks) => Task.CompletedTask;
    }

}
