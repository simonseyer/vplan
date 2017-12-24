using System;
using System.Collections.Generic;
using System.Collections.Immutable;
namespace FLSVertretungsplan
{
    public class VplanDiff
    {
        public bool Updated;
        public ImmutableArray<Change> NewBookmarkedChanges { get; private set; }
        public ImmutableArray<SchoolClassBookmark> NewNewSchoolClassBookmarks { get; private set; }

        public VplanDiff(bool updated, List<Change> newBookmarkedChanges, List<SchoolClassBookmark> newNewClassBookmarks)
        {
            Updated = updated;
            NewBookmarkedChanges = ImmutableArray.CreateRange(newBookmarkedChanges);
            NewNewSchoolClassBookmarks = ImmutableArray.CreateRange(newNewClassBookmarks);
        }
    }
}
