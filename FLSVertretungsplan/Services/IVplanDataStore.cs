using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FLSVertretungsplan
{
    public interface IVplanDataStore
    {

        Task<Vplan> GetVplanAsync(bool forceRefresh = false);

    }
}
