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
    [Register ("ChangeTableViewCell")]
    partial class ChangeTableViewCell
    {
        [Outlet]
        public UIKit.UILabel ChangeDescriptionLabel { get; private set; }


        [Outlet]
        public UIKit.UILabel ChangeLabel { get; private set; }


        [Outlet]
        public UIKit.UIView ContentBackgroundView { get; private set; }


        [Outlet]
        public UIKit.UILabel HoursLabel { get; private set; }


        [Outlet]
        public UIKit.UILabel OriginalLessonLabel { get; private set; }


        [Outlet]
        public UIKit.UILabel SchoolClassLabel { get; private set; }


        [Outlet]
        public FLSVertretungsplan.iOS.ChipCell SchoolGradientView { get; private set; }

        void ReleaseDesignerOutlets ()
        {
            if (ChangeDescriptionLabel != null) {
                ChangeDescriptionLabel.Dispose ();
                ChangeDescriptionLabel = null;
            }

            if (ChangeLabel != null) {
                ChangeLabel.Dispose ();
                ChangeLabel = null;
            }

            if (ContentBackgroundView != null) {
                ContentBackgroundView.Dispose ();
                ContentBackgroundView = null;
            }

            if (HoursLabel != null) {
                HoursLabel.Dispose ();
                HoursLabel = null;
            }

            if (OriginalLessonLabel != null) {
                OriginalLessonLabel.Dispose ();
                OriginalLessonLabel = null;
            }

            if (SchoolClassLabel != null) {
                SchoolClassLabel.Dispose ();
                SchoolClassLabel = null;
            }

            if (SchoolGradientView != null) {
                SchoolGradientView.Dispose ();
                SchoolGradientView = null;
            }
        }
    }
}