using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace FLSVertretungsplan
{
    public class StandardSettingsDataStore: ISettingsDataStore
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        private const string InitialStartKey = "InitialStartKey";
        public bool FirstAppLaunch
        {
            get
            {
                return AppSettings.GetValueOrDefault(InitialStartKey, true);
            }
            set
            {
                AppSettings.AddOrUpdateValue(InitialStartKey, value);
            }
        }
    }
}
