using System;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public interface IVplanLoader
    {

        Task<Vplan> Load();

    }
}
