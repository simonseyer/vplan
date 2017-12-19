using System;
using UIKit;

namespace FLSVertretungsplan.iOS
{

    static class RgbColorExtension
    {
        public static UIColor ToUIColor(this RgbColor color)
        {
            return UIColor.FromRGB(color.Red, color.Green, color.Blue);
        }
    }
}
