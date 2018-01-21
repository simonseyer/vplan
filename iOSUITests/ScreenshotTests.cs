using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;
using Xamarin.UITest.Queries;

namespace iOS.uitest
{
    [TestFixture]
    public class ScreenshotTests
    {
        iOSApp app;

        static ScreenshotTests()
        {
            LoadAllDevices();
        }

        [SetUp]
        public void BeforeEachTest()
        {
            
        }

        static string[] Devices = new string[]
        {
            "iPhone X (11.2)",
            "iPhone 8 Plus (11.2)",
            "iPhone 8 (11.2)",
            "iPhone 5s (11.2)"
        };

        static Dictionary<string, string> AllDevices;
        static void LoadAllDevices()
        {
            var proc = new Process();
            proc.StartInfo.FileName = @"xcrun";
            proc.StartInfo.Arguments = "instruments -s devices";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();

            var output = proc.StandardOutput.ReadToEnd();

            proc.WaitForExit();
            var exitCode = proc.ExitCode;
            proc.Close();

            var devices = new Dictionary<string, string>();
            foreach (string device in output.Split("\n".ToCharArray()))
            {
                var components = device.Split('[');
                if (components.Count() != 2)
                {
                    continue;
                }
                var name = components[0].Trim();
                var id = components[1].Split(']')[0];
                devices[name] = id;
            }

            AllDevices = devices;
        }

        [Test]
        public void AppLaunches()
        {
            foreach (string device in Devices)
            {
                app = ConfigureApp
                .iOS
                .AppBundle("../../../iOS/bin/iPhoneSimulator/Debug/FLSVertretungsplan.iOS.app")
                .DeviceIdentifier(AllDevices[device])
                .EnableLocalScreenshots()
                .StartApp();
                app.Invoke("activateTestEnvironment:", "true");

                app.WaitForElement(c => c.Marked("Auf geht's"));
                app.Tap(c => c.Marked("Auf geht's"));

                app.Screenshot("no-changes");

                app.Tap(c => c.Marked("Filter"));
                app.Tap(c => c.Marked("BFS"));
                app.Tap(c => c.Marked("BG"));
                app.Tap(c => c.Marked("HBFS"));

                app.Tap(c => c.Marked("1082"));
                app.Tap(c => c.Marked("11/4 W"));
                app.Tap(c => c.Marked("11/11 G"));
                app.ScrollDown("SchoolClassCollectionView");
                app.Tap(c => c.Marked("12"));
                app.Tap(c => c.Marked("1292"));

                app.Screenshot("filter");

                app.Tap(c => c.Marked("Mein Plan"));

                app.Screenshot("my-plan");

                app.Tap(c => c.Marked("Schul-Plan"));
                app.SwipeRightToLeft();

                app.Screenshot("school-plan");

                app.TouchAndHold("Deutsch bei P. Pohl in Raum 301");

                app.Screenshot("sharing");

                var path = "../../../iOS/metadata/screenshots/de-DE/" + device;
                try
                {
                    Directory.Delete(path);
                } 
                catch (IOException) { }
                Directory.CreateDirectory(path);
                for (var i = 1; i <= 5; i++)
                {
                    try
                    {
                        var oldPath = string.Format("screenshot-{0}.png", i);
                        var newPath = string.Format("{0}/screenshot-{1}.png", path, i);
                        try
                        {
                            File.Delete(newPath);
                        }
                        catch (IOException) {}
                        File.Move(oldPath, newPath);
                    }
                    catch (Exception e)
                    {
                        Debug.Print(e.Message);
                    }
                }
            }
        }
    }
}
