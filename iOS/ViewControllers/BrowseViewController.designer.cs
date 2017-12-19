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
    [Register ("ItemsViewController")]
    partial class BrowseViewController
    {
        [Outlet]
        UIKit.UILabel LastUpdateLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (LastUpdateLabel != null) {
                LastUpdateLabel.Dispose ();
                LastUpdateLabel = null;
            }
        }
    }
}