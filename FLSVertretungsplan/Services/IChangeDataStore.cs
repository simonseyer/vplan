using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public interface IChangeDataStore
    {

        Task<IEnumerable<Change>> GetChangesAsync(bool forceRefresh = false);

    }
}
