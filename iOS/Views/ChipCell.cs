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

        public Gradient gradient;
        public Gradient Gradient
        {
            get
            {
                return gradient;
            }
            set
            {
                gradient = value;
                if (GradientLayer == null)
                {
                    GradientLayer = new CAGradientLayer
                    {
                        Locations = new NSNumber[] { 0, 1 },
                        Frame = Bounds
                    };
                    GradientLayer.Transform = CATransform3D.MakeRotation(NMath.PI / 2, 0, 0, 1);
                    Layer.InsertSublayer(GradientLayer, 0);
                }
                if (gradient != null)
                {
                    GradientLayer.Colors = new CGColor[] { gradient.Start.ToUIColor().CGColor, gradient.End.ToUIColor().CGColor };
                }
                else
                {
                    GradientLayer.Colors = new CGColor[] { UIColor.Clear.CGColor, UIColor.Clear.CGColor };
                }
            }
        }

        protected ChipCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }


        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (GradientLayer != null)
            {
                CATransaction.Begin();
                CATransaction.DisableActions = true;
                GradientLayer.Frame = Bounds;
                CATransaction.Commit();
            }
        }

    }
}
