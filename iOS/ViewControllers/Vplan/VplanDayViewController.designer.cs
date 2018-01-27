// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
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

		[Outlet]
		UIKit.NSLayoutConstraint TopMarginConstraint { get; set; }
		
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

			if (TopMarginConstraint != null) {
				TopMarginConstraint.Dispose ();
				TopMarginConstraint = null;
			}
		}
	}
}
