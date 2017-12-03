// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace FLSVertretungsplan.iOS.Views
{
	[Register ("ChangeCell")]
	partial class ChangeCell
	{
		[Outlet]
		public UIKit.UILabel ChangeDescriptionLabel { get; private set; }

		[Outlet]
		public UIKit.UILabel ChangeTypeLabel { get; private set; }

		[Outlet]
		public UIKit.UILabel ClassNameLabel { get; private set; }

		[Outlet]
		public UIKit.UILabel HoursLabel { get; private set; }

		[Outlet]
		public UIKit.UILabel OriginalLessonLabel { get; private set; }

		[Outlet]
        public UIKit.UIView SchoolView { get; set; }

		[Outlet]
        public UIKit.UIView StatusView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ChangeDescriptionLabel != null) {
				ChangeDescriptionLabel.Dispose ();
				ChangeDescriptionLabel = null;
			}

			if (ChangeTypeLabel != null) {
				ChangeTypeLabel.Dispose ();
				ChangeTypeLabel = null;
			}

			if (ClassNameLabel != null) {
				ClassNameLabel.Dispose ();
				ClassNameLabel = null;
			}

			if (HoursLabel != null) {
				HoursLabel.Dispose ();
				HoursLabel = null;
			}

			if (OriginalLessonLabel != null) {
				OriginalLessonLabel.Dispose ();
				OriginalLessonLabel = null;
			}

			if (StatusView != null) {
				StatusView.Dispose ();
				StatusView = null;
			}

			if (SchoolView != null) {
				SchoolView.Dispose ();
				SchoolView = null;
			}
		}
	}
}
