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
    [Register ("VplanContainerViewController")]
    partial class VplanContainerViewController
    {
        [Outlet]
        public UIKit.UIPageControl PageControl { get; private set; }

        [Outlet]
        public UIKit.UIScrollView ScrollView { get; private set; }

        [Outlet]
        public UIKit.NSLayoutConstraint StatusViewHiddenConstraint { get; private set; }

        void ReleaseDesignerOutlets ()
        {
            if (PageControl != null) {
                PageControl.Dispose ();
                PageControl = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }

            if (StatusViewHiddenConstraint != null) {
                StatusViewHiddenConstraint.Dispose ();
                StatusViewHiddenConstraint = null;
            }
        }
    }
}