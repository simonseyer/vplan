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
                ServiceLocator.Instance.Register<IChangeDataStore, MockChangeDataStore>();
            }
            else
            {
                ServiceLocator.Instance.Register<IChangeDataStore, CloudChangeDataStore>();
            }
        }
    }
}
