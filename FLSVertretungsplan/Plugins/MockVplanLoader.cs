using System;
using System.Threading.Tasks;

namespace FLSVertretungsplan
{
    public class MockVplanLoader: IVplanLoader
    {
        private IVplanPersistence Persistence => ServiceLocator.Instance.Get<IVplanPersistence>();

        public async Task<Vplan> Load()
        {
            return await Persistence.LoadVplan();
        }
    }
}
