using System.Collections.Generic;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public interface IVplanPersistence
    {
        Task PersistVplan(Vplan vplan);
        Task PersistSchoolBookmarks(List<SchoolBookmark> schoolBookmarks);
        Task PersistSchoolClassBookmarks(List<SchoolClassBookmark> schoolClassBookmarks);
        Task PersistNewSchoolClassBookmarks(List<SchoolClassBookmark> newSchoolClassBookmarks);
        Task PersistTeacherBookmarks(List<TeacherBookmark> teacherBookmarks);

        Task<Vplan> LoadVplan();
        Task<List<SchoolBookmark>> LoadSchoolBookmarks();
        Task<List<SchoolClassBookmark>> LoadSchoolClassBookmarks();
        Task<List<SchoolClassBookmark>> LoadNewSchoolClassBookmarks();
        Task<List<TeacherBookmark>> LoadTeacherBookmarks();
    }
}
