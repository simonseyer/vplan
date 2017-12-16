using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace FLSVertretungsplan
{
    public class Vplan
    {
        public DateTime LastUpdate { get; private set; }
        public ImmutableArray<Change> Changes { get; private set; }

        public Vplan(DateTime lastUpdate, List<Change> changes)
        {
            LastUpdate = lastUpdate;
            Changes = ImmutableArray.CreateRange(changes);
        }

        public override bool Equals(object obj)
        {
            var vplan = obj as Vplan;
            return vplan != null &&
                   LastUpdate == vplan.LastUpdate &&
                   Changes.Equals(vplan.Changes);
        }

        public override int GetHashCode()
        {
            var hashCode = -741907486;
            hashCode = hashCode * -1521134295 + LastUpdate.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ImmutableArray<Change>>.Default.GetHashCode(Changes);
            return hashCode;
        }
    }
}
