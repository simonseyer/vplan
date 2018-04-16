using System;

using Foundation;
using ObjCRuntime;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class ChangeTableViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("ChangeTableViewCell");
        public static readonly UINib Nib;

        static ChangeTableViewCell()
        {
            Nib = UINib.FromName("ChangeTableViewCell", NSBundle.MainBundle);
        }

        protected ChangeTableViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public static ChangeTableViewCell Create()
        {
            var arr = NSBundle.MainBundle.LoadNib("ChangeTableViewCell", null, null);
            var v = Runtime.GetNSObject<ChangeTableViewCell>(arr.ValueAt(0));

            return v;
        }

        public override void AwakeFromNib()
        {
            ContentBackgroundView.Layer.CornerRadius = 8;
            ContentBackgroundView.Layer.BorderWidth = 1;
            ContentBackgroundView.Layer.BorderColor = Color.BORDER_COLOR.ToUIColor().CGColor;
            ContentBackgroundView.Layer.ShadowColor = UIColor.Black.CGColor;
            ContentBackgroundView.Layer.ShadowOffset = new CoreGraphics.CGSize(0, 2);
            ContentBackgroundView.Layer.ShadowRadius = 3;
            ContentBackgroundView.Layer.ShadowOpacity = 0.06F;

            SchoolClassLabel.TextColor = Color.PRIMARY_TEXT_COLOR.ToUIColor();
            HoursLabel.TextColor = Color.PRIMARY_TEXT_COLOR.ToUIColor();
            ChangeLabel.TextColor = Color.PRIMARY_TEXT_COLOR.ToUIColor();
        }

        public void DisableShadows()
        {
            ContentBackgroundView.Layer.ShadowOpacity = 0;
            ContentBackgroundView.Layer.BorderWidth = 0;
        }

        public void DisableRoundedCorners()
        {
            ContentBackgroundView.Layer.CornerRadius = 0;
        }
    }
}
