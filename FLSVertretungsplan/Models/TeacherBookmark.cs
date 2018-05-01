using System;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public class TeacherBookmark: IComparable
    {
        public Teacher Teacher { get; private set; }
        public bool Bookmarked { get; private set; }

        public TeacherBookmark(Teacher teacher, bool bookmarked)
        {
            Teacher = teacher;
            Bookmarked = bookmarked;
        }

        public override bool Equals(object obj)
        {
            return obj is TeacherBookmark bookmark &&
                   EqualityComparer<Teacher>.Default.Equals(Teacher, bookmark.Teacher);
        }

        public override int GetHashCode()
        {
            return -124588675 + EqualityComparer<Teacher>.Default.GetHashCode(Teacher);
        }

        public int CompareTo(object obj)
        {
            if (obj is TeacherBookmark bookmark)
            {
                return Teacher.CompareTo(bookmark.Teacher);
            }
            throw new ArgumentException("Object is not a TeacherBookmark");
        }
    }
}
