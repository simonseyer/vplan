using System;
namespace FLSVertretungsplan
{
    public interface ISettingsDataStore
    {

        bool FirstAppLaunch { get; set; }

        bool TestEnvironment { get; set; }
    }
}
