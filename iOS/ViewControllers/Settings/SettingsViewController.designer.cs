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
	[Register ("SettingsViewController")]
	partial class SettingsViewController
	{
		[Outlet]
		UIKit.UICollectionView SchoolClassCollectionView { get; set; }

		[Outlet]
		UIKit.UILabel SchoolClassSectionLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint SchoolClassTitleTopMarginConstraint { get; set; }

		[Outlet]
		UIKit.UICollectionView SchoolCollectionView { get; set; }

		[Outlet]
		UIKit.UILabel SchoolSectionLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint SchoolTitleTopMarginConstraint { get; set; }

		[Outlet]
		UIKit.UILabel SubTitleLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint TopMarginConstraint { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SchoolClassCollectionView != null) {
				SchoolClassCollectionView.Dispose ();
				SchoolClassCollectionView = null;
			}

			if (SchoolClassSectionLabel != null) {
				SchoolClassSectionLabel.Dispose ();
				SchoolClassSectionLabel = null;
			}

			if (SchoolCollectionView != null) {
				SchoolCollectionView.Dispose ();
				SchoolCollectionView = null;
			}

			if (SchoolSectionLabel != null) {
				SchoolSectionLabel.Dispose ();
				SchoolSectionLabel = null;
			}

			if (SubTitleLabel != null) {
				SubTitleLabel.Dispose ();
				SubTitleLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TopMarginConstraint != null) {
				TopMarginConstraint.Dispose ();
				TopMarginConstraint = null;
			}

			if (SchoolTitleTopMarginConstraint != null) {
				SchoolTitleTopMarginConstraint.Dispose ();
				SchoolTitleTopMarginConstraint = null;
			}

			if (SchoolClassTitleTopMarginConstraint != null) {
				SchoolClassTitleTopMarginConstraint.Dispose ();
				SchoolClassTitleTopMarginConstraint = null;
			}
		}
	}
}
