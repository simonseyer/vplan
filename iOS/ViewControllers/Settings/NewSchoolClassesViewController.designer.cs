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
            if (CollectionView != null) {
                CollectionView.Dispose ();
                CollectionView = null;
            }

            if (DoneButtonLabel != null) {
                DoneButtonLabel.Dispose ();
                DoneButtonLabel = null;
            }

            if (SubTitleLabel != null) {
                SubTitleLabel.Dispose ();
                SubTitleLabel = null;
            }

            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }
        }
    }
}