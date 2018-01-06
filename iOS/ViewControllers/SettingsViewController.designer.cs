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
    [Register ("SettingsViewController")]
    partial class SettingsViewController
    {
        [Outlet]
        UIKit.UICollectionView SchoolClassCollectionView { get; set; }


        [Outlet]
        UIKit.UILabel SchoolClassSectionLabel { get; set; }


        [Outlet]
        UIKit.UICollectionView SchoolCollectionView { get; set; }


        [Outlet]
        UIKit.UILabel SchoolSectionLabel { get; set; }


        [Outlet]
        UIKit.UILabel SubTitleLabel { get; set; }


        [Outlet]
        UIKit.UILabel TitleLabel { get; set; }

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
        }
    }
}