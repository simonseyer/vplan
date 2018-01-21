using System;
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

        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp
                .iOS
                .EnableLocalScreenshots()
                .StartApp();
            app.Invoke("activateTestEnvironment:", "true");
        }

        [Test]
        public void AppLaunches()
        {
            app.WaitForElement(c => c.Marked("Auf geht's"));
            app.Tap(c => c.Marked("Auf geht's"));

            app.Screenshot("no-changes");

            app.Tap(c => c.Marked("Filter"));
            app.Tap(c => c.Marked("BFS"));
            app.Tap(c => c.Marked("BG"));
            app.Tap(c => c.Marked("HBFS"));
            app.Tap(c => c.Marked("1082"));
            app.Tap(c => c.Marked("11/4 W"));
            app.Tap(c => c.Marked("12"));
            app.Tap(c => c.Marked("11/11 G"));
            app.Tap(c => c.Marked("1292"));

            app.Screenshot("filter");

            app.Tap(c => c.Marked("Mein Plan"));

            app.Screenshot("my-plan");

            app.Tap(c => c.Marked("Schul-Plan"));
            app.SwipeRightToLeft();

            app.Screenshot("school-plan");

            app.TouchAndHold("Deutsch bei P. Pohl in Raum 301");

            app.Screenshot("sharing");
        }
    }
}
