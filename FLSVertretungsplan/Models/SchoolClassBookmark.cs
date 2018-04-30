using System;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public class SchoolClassBookmark: IComparable
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
            return obj is SchoolClassBookmark bookmark &&
                   EqualityComparer<SchoolClass>.Default.Equals(SchoolClass, bookmark.SchoolClass);
        }

        public override int GetHashCode()
        {
            return 573291979 + EqualityComparer<SchoolClass>.Default.GetHashCode(SchoolClass);
        }

        public int CompareTo(object obj)
        {
            if (obj is SchoolClassBookmark bookmark)
            {
                return SchoolClass.CompareTo(bookmark.SchoolClass);
            }
            throw new ArgumentException("Object is not a SchoolClass");
        }
    }
}
