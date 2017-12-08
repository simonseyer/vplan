using System;

namespace FLSVertretungsplan
{
    public static class App
    {
        public static bool UseMockDataStore = false;
        public static string BackendUrl = "https://fls-wiesbaden.de";

        public static void Initialize()
        {
            if (UseMockDataStore)
            {
                ServiceLocator.Instance.Register<IVplanDataStore, MockVplanDataStore>();
            }
            else
            {
                ServiceLocator.Instance.Register<IVplanDataStore, CloudVplanDataStore>();
            }
        }
    }
}
