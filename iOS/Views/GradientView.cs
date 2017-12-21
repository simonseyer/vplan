using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    [Register("GradientView")]
    public class GradientView : UIView
    {
        CAGradientLayer GradientLayer;

        public Gradient _Gradient;
        public Gradient Gradient
        {
            get
            {
                return _Gradient;
            }
            set
            {
                _Gradient = value;
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
                if (_Gradient != null)
                {
                    GradientLayer.Colors = new CGColor[] { _Gradient.Start.ToUIColor().CGColor, _Gradient.End.ToUIColor().CGColor };
                }
                else
                {
                    GradientLayer.Colors = new CGColor[] { UIColor.Clear.CGColor, UIColor.Clear.CGColor };
                }
            }
        }

        protected GradientView(IntPtr handle) : base(handle)
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
