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
    [Register ("SetupViewController")]
    partial class SetupViewController
    {
        [Outlet]
        UIKit.UILabel StartButtonLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (StartButtonLabel != null) {
                StartButtonLabel.Dispose ();
                StartButtonLabel = null;
            }
        }
    }
}