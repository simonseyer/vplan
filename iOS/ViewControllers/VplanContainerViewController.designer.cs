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
