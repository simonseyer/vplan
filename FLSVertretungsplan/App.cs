using System;

namespace FLSVertretungsplan
{
    public static class App
    {
        public static bool UseMockDataStore = false;
        public static string BackendUrl = "https://fls-wiesbaden.de";

        public static void Initialize()
        {
            ServiceLocator.Instance.Register<IVplanDataStore, CloudVplanDataStore>();

            if (UseMockDataStore)
            {
                ServiceLocator.Instance.Register<IVplanLoader, MockVplanLoader>();
                ServiceLocator.Instance.Register<IVplanPersistence, MockVplanPersistence>();
            }
            else
            {
                ServiceLocator.Instance.Register<IVplanLoader, CloudVplanLoader>();
                ServiceLocator.Instance.Register<IVplanPersistence, JsonVplanPersistence>();
            }
        }
    }
}
