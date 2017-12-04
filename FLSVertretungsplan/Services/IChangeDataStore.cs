using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public interface IChangeDataStore
    {

        Task<Vplan> GetVplanAsync(bool forceRefresh = false);

    }
}
