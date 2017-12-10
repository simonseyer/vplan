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
		UIKit.UICollectionView classCollectionView { get; set; }

		[Outlet]
		UIKit.UICollectionView schoolCollectionView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (schoolCollectionView != null) {
				schoolCollectionView.Dispose ();
				schoolCollectionView = null;
			}

			if (classCollectionView != null) {
				classCollectionView.Dispose ();
				classCollectionView = null;
			}
		}
	}
}
