using System;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace FLSVertretungsplan
{
    public class StandardSettingsDataStore: ISettingsDataStore
    {
        static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        const string InitialStartKey = "InitialStartKey";
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

        bool _TestEnvironment;
        public bool TestEnvironment
        {
            get
            {
                return _TestEnvironment;
            }
            set
            {
                _TestEnvironment = value;
            }
        }
    }
}
