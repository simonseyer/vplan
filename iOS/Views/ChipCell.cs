using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class ChipCell : UICollectionViewCell
    {
        CAGradientLayer GradientLayer;

        Gradient gradient;
        public Gradient Gradient
        {
            get
            {
                return gradient;
            }
            set
            {
                gradient = value;
                CATransaction.Begin();
                CATransaction.DisableActions = true;
                if (gradient != null)
                {
                    GradientLayer.Colors = new CGColor[] { gradient.Start.ToUIColor().CGColor, gradient.End.ToUIColor().CGColor };
                }
                else
                {
                    GradientLayer.Colors = new CGColor[] { UIColor.Clear.CGColor, UIColor.Clear.CGColor };
                }
                CATransaction.Commit();
            }
        }

        protected ChipCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            GradientLayer = new CAGradientLayer
            {
                Locations = new NSNumber[] { 0, 1 },
                Transform = CATransform3D.MakeRotation(NMath.PI / 2, 0, 0, 1)
            };
            Layer.InsertSublayer(GradientLayer, 0);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CATransaction.Begin();
            CATransaction.DisableActions = true;
            GradientLayer.Frame = Bounds;
            CATransaction.Commit();
        }

    }
}
