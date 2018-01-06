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
    [Register ("EmptyVplanViewController")]
    partial class EmptyVplanViewController
    {
        [Outlet]
        UIKit.UIScrollView ScrollView { get; set; }


        [Outlet]
        UIKit.UILabel SubTitleLabel { get; set; }


        [Outlet]
        UIKit.UILabel TitleLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
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