// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace FLSVertretungsplan.iOS
{
    [Register ("VplanDayViewController")]
    partial class VplanDayViewController
    {
        [Outlet]
        UIKit.UILabel DaySubTitleLabel { get; set; }


        [Outlet]
        UIKit.UILabel DayTitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DaySubTitleLabel != null) {
                DaySubTitleLabel.Dispose ();
                DaySubTitleLabel = null;
            }

            if (DayTitleLabel != null) {
                DayTitleLabel.Dispose ();
                DayTitleLabel = null;
            }
        }
    }
}