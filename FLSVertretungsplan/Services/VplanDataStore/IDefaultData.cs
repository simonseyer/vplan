using System;
using System.Collections.ObjectModel;

namespace FLSVertretungsplan
{
    public interface IDefaultData
    {
        Collection<string> Schools { get; }
        Collection<SchoolClass> SchoolClasses { get; }
    }
}
