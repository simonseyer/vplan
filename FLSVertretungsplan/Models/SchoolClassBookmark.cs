using System;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public class SchoolClassBookmark
    {
        public SchoolClass SchoolClass { get; private set; }
        public bool Bookmarked { get; private set; }
        public bool SchoolBookmarked { get; private set; }

        public SchoolClassBookmark(SchoolClass schoolClass, bool bookmarked, bool schoolBookmarked)
        {
            SchoolClass = schoolClass;
            Bookmarked = bookmarked;
            SchoolBookmarked = schoolBookmarked;
        }

        public override bool Equals(object obj)
        {
            var bookmark = obj as SchoolClassBookmark;
            return bookmark != null &&
                   EqualityComparer<SchoolClass>.Default.Equals(SchoolClass, bookmark.SchoolClass);
        }

        public override int GetHashCode()
        {
            return 573291979 + EqualityComparer<SchoolClass>.Default.GetHashCode(SchoolClass);
        }
    }
}
