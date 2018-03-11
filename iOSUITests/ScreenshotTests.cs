using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.iOS;

namespace iOS.uitest
{
    [TestFixture]
    public class ScreenshotTests
    {
        static ScreenshotTests()
        {
            LoadAllDevices(OSVersion);
        }

        [SetUp]
        public void BeforeEachTest()
        {
            
        }

        static readonly string[] Devices = new string[]
        {
            "iPhone X",
            "iPhone 8 Plus",
            "iPhone 8",
            "iPhone 5s"
        };
        static readonly string OSVersion = "11.2";
        static readonly string BundlePath = "../../../iOS/bin/iPhoneSimulator/Debug/FLSVertretungsplan.iOS.app";
        static readonly string TargetPath = "../../../iOS/fastlane/screenshots/de-DE/";

        [Test]
        public void ProcessAllDevices()
        {
            foreach (string device in Devices)
            {
                iOSApp app = PrepareDevice(device);
                TestApp(app);
                CollectScreenshots(device);
            }
        }

        static void TestApp(iOSApp app)
        {
            app.WaitForElement(c => c.Marked("Auf geht's"));
            app.Tap(c => c.Marked("Auf geht's"));

            app.Screenshot("no-changes");

            app.Tap(c => c.Marked("Filter"));
            app.Tap(c => c.Marked("BFS"));
            app.Tap(c => c.Marked("BG"));
            app.Tap(c => c.Marked("HBFS"));

            app.Tap(c => c.Marked("1082"));
            app.Tap(c => c.Marked("11/12 M"));
            app.Tap(c => c.Marked("11/11 G"));

            app.ScrollDown("SchoolClassCollectionView", strategy: ScrollStrategy.Gesture);

            app.Tap(c => c.Marked("12"));
            app.Tap(c => c.Marked("1292"));

            app.ScrollUp("SchoolClassCollectionView", strategy: ScrollStrategy.Gesture);

            app.Screenshot("filter");

            app.Tap(c => c.Marked("Mein Plan"));

            app.Screenshot("my-plan");

            app.Tap(c => c.Marked("Schul-Plan"));
            app.SwipeRightToLeft();

            app.Screenshot("school-plan");

            app.TouchAndHold("Deutsch bei P. Pohl in Raum 301");

            app.Screenshot("sharing");
        }

        static iOSApp PrepareDevice(string device)
        {
            Assert.NotNull(AllDevices[device], "Device " + device + " not found");

            iOSApp app = ConfigureApp
                .iOS
                .AppBundle(BundlePath)
                .DeviceIdentifier(AllDevices[device])
                .EnableLocalScreenshots()
                .StartApp();
            
            app.Invoke("activateTestEnvironment:", "true");

            return app;
        }

        static void CollectScreenshots(string device)
        {
            Directory.CreateDirectory(TargetPath);

            var deviceName = device.Replace(' ', '-');
            for (var i = 1; i <= 5; i++)
            {
                try
                {
                    var oldPath = string.Format("screenshot-{0}.png", i);
                    var newPath = string.Format("{0}/{1}-{2}.png", TargetPath, deviceName, i);
                    try
                    {
                        File.Delete(newPath);
                    }
                    catch (IOException) { }
                    File.Move(oldPath, newPath);
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                }
            }
        }

        static Dictionary<string, string> AllDevices;
        static void LoadAllDevices(string osVersion)
        {
            /* Matches:
             * iPhone 8 Plus (11.2) [06E55EA2-AE29-4D72-83DD-122931620569] (Simulator)
             * -------------  ----   ------------------------------------
             * 1              2      3
             * 
             * Doesn't match:
             * iPhone 8 Plus (11.2) + Apple Watch Series 3 - 42mm (4.2) [35BEAFCB-78E6-4067-B551-C5897F8B5644] (Simulator)
             */
            var pattern = @"^([\w\s]*)\s\(([0-9.]+)\)\s\[([\w-]*)\]";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            var devices = new Dictionary<string, string>();
            foreach (string device in LoadDeviceList())
            {
                Match match = regex.Match(device);
                if (!match.Success || match.Groups[2].Value != osVersion)
                {
                    continue;
                }

                string name = match.Groups[1].Value.Trim();
                string id = match.Groups[3].Value;
                devices[name] = id;
            }

            AllDevices = devices;
        }

        static string[] LoadDeviceList()
        {
            var proc = new Process();
            proc.StartInfo.FileName = @"xcrun";
            proc.StartInfo.Arguments = "instruments -s devices";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();

            var output = proc.StandardOutput.ReadToEnd();

            proc.WaitForExit();
            proc.Close();

            return output.Split(Environment.NewLine.ToCharArray());
        }
    }
}
