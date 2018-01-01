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
    [Register ("NewSchoolClassesViewController")]
    partial class NewSchoolClassesViewController
    {
        [Outlet]
        UIKit.UICollectionView CollectionView { get; set; }

        [Outlet]
        UIKit.UILabel DoneButtonLabel { get; set; }

        [Outlet]
        UIKit.UILabel SubTitleLabel { get; set; }

        [Outlet]
        UIKit.UILabel TitleLabel { get; set; }

        [Action ("CloseTapped:")]
        partial void CloseTapped (Foundation.NSObject sender);

        [Action ("DoneTapped:")]
        partial void DoneTapped (Foundation.NSObject sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }

            if (SubTitleLabel != null) {
                SubTitleLabel.Dispose ();
                SubTitleLabel = null;
            }

            if (CollectionView != null) {
                CollectionView.Dispose ();
                CollectionView = null;
            }

            if (DoneButtonLabel != null) {
                DoneButtonLabel.Dispose ();
                DoneButtonLabel = null;
            }
        }
    }
}
