using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class ChipCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString("ChipCollectionViewCell");

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
                if (gradient != null)
                {
                    UpdateColors(new CGColor[] { gradient.Start.ToUIColor().CGColor, gradient.End.ToUIColor().CGColor });
                }
                else
                {
                    UpdateColors(new CGColor[] { UIColor.Clear.CGColor, UIColor.Clear.CGColor });
                }
            }
        }

        protected ChipCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            GradientLayer = new CAGradientLayer
            {
                Locations = new NSNumber[] { 0, 1 },
                // Rotate gradient to be horizontal
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

        void UpdateColors(CGColor[] colors)
        {
            CATransaction.Begin();
            CATransaction.DisableActions = true;
            GradientLayer.Colors = colors;
            CATransaction.Commit();
        }

    }
}
