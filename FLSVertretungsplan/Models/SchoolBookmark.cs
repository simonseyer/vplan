using System;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public class SchoolBookmark
    {
        public string School { get; private set; }
        public bool Bookmarked { get; private set; }

        public SchoolBookmark(string school, bool bookmarked)
        {
            School = school;
            Bookmarked = bookmarked;
        }

        public override bool Equals(object obj)
        {
            var bookmark = obj as SchoolBookmark;
            return bookmark != null &&
                   School == bookmark.School;
        }

        public override int GetHashCode()
        {
            return -39790255 + EqualityComparer<string>.Default.GetHashCode(School);
        }
    }
}
