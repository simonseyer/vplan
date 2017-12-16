using System;
using System.Collections.Generic;
using System.Collections.Immutable;
namespace FLSVertretungsplan
{
    public class VplanDiff
    {
        public bool Updated;
        public ImmutableArray<Change> NewBookmarkedChanges { get; private set; }
        public ImmutableArray<SchoolClass> NewNewClasses { get; private set; }

        public VplanDiff(bool updated, List<Change> newBookmarkedChanges, List<SchoolClass> newNewClasses)
        {
            Updated = updated;
            NewBookmarkedChanges = ImmutableArray.CreateRange(newBookmarkedChanges);
            NewNewClasses = ImmutableArray.CreateRange(newNewClasses);
        }
    }
}
