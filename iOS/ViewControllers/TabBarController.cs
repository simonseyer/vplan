using System;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class TabBarController : UITabBarController
    {
        public TabBarController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TabBar.Layer.BorderColor = UIColor.FromRGB(236, 237, 241).CGColor;
            TabBar.Layer.BorderWidth = (nfloat)0.5;
            TabBar.ClipsToBounds = true;
            TabBar.TintColor = UIColor.FromRGB(23, 43, 76);
            TabBar.UnselectedItemTintColor = UIColor.FromRGB(164, 174, 186);
        }
    }
}

