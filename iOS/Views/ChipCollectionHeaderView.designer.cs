﻿// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
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