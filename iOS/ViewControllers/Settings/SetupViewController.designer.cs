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
