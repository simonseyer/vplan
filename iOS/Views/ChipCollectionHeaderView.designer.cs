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
    [Register ("ChipCollectionHeaderView")]
    partial class ChipCollectionHeaderView
    {
        [Outlet]
        public UIKit.UILabel TitleLabel { get; set; }

        [Outlet]
        public UIKit.NSLayoutConstraint TitleTopMarginConstraint { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (TitleLabel != null) {
                TitleLabel.Dispose ();
                TitleLabel = null;
            }

            if (TitleTopMarginConstraint != null) {
                TitleTopMarginConstraint.Dispose ();
                TitleTopMarginConstraint = null;
            }
        }
    }
}