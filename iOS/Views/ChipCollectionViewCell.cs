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

        Gradient _Gradient;
        public Gradient Gradient
        {
            get
            {
                return _Gradient;
            }
            set
            {
                _Gradient = value;
                UpdateColors();
            }
        }

        UIColor _OutlineColor;
        public UIColor OutlineColor
        {
            get
            {
                return _OutlineColor;
            }
            set
            {
                _OutlineColor = value;
                UpdateColors();

            }
        }

        bool _Outline;
        public bool Outline
        {
            get
            {
                return _Outline;    
            }
            set
            {
                _Outline = value;
                UpdateColors();
            }
        }

        protected ChipCollectionViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            ClipsToBounds = true;
            GradientLayer = new CAGradientLayer
            {
                Locations = new NSNumber[] { 0, 1 },
                // Rotate gradient to be horizontal
                Transform = CATransform3D.MakeRotation(- NMath.PI / 2, 0, 0, 1)
            };
            Layer.InsertSublayer(GradientLayer, 0);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CATransaction.Begin();
            CATransaction.DisableActions = true;
            GradientLayer.Frame = Bounds;
            Layer.CornerRadius = Bounds.Height / 2;
            CATransaction.Commit();
        }

        void UpdateColors()
        {
            CGColor[] gradientColors;
            if (_Gradient != null && !Outline)
            {
                gradientColors = new CGColor[] { _Gradient.Start.ToUIColor().CGColor, _Gradient.End.ToUIColor().CGColor };
            }
            else
            {
                gradientColors = new CGColor[] { UIColor.Clear.CGColor, UIColor.Clear.CGColor };
            }

            CATransaction.Begin();
            CATransaction.DisableActions = true;
            if (Outline)
            {
                Layer.BorderWidth = 1;
                Layer.BorderColor = OutlineColor.CGColor;
                if (Label != null)
                {
                    Label.TextColor = OutlineColor;
                }
            }
            else
            {
                Layer.BorderWidth = 0;
                Layer.BorderColor = UIColor.Clear.CGColor;
                if (Label != null)
                {
                    Label.TextColor = UIColor.White;
                }
            }
            GradientLayer.Colors = gradientColors;
            CATransaction.Commit();
        }
    }
}
