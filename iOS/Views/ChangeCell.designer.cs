// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace FLSVertretungsplan.iOS.Views
{
    [Register ("ChangeCell")]
    partial class ChangeCell
    {
        [Outlet]
        public UIKit.UILabel ChangeDescriptionLabel { get; private set; }

        [Outlet]
        public UIKit.UILabel ChangeTypeLabel { get; private set; }

        [Outlet]
        public UIKit.UILabel ClassNameLabel { get; private set; }

        [Outlet]
        public UIKit.UIView ContentBackgroundView { get; private set; }

        [Outlet]
        public UIKit.UILabel HoursLabel { get; private set; }

        [Outlet]
        public UIKit.UILabel OriginalLessonLabel { get; private set; }

        [Outlet]
        public FLSVertretungsplan.iOS.GradientView SchoolView { get; private set; }

        void ReleaseDesignerOutlets ()
        {
            if (ChangeDescriptionLabel != null) {
                ChangeDescriptionLabel.Dispose ();
                ChangeDescriptionLabel = null;
            }

            if (ChangeTypeLabel != null) {
                ChangeTypeLabel.Dispose ();
                ChangeTypeLabel = null;
            }

            if (ClassNameLabel != null) {
                ClassNameLabel.Dispose ();
                ClassNameLabel = null;
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

            if (SchoolView != null) {
                SchoolView.Dispose ();
                SchoolView = null;
            }
        }
    }
}