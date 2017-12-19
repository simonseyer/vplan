using System;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    [Register ("DynamicCollectionView")]
    public class DynamicCollectionView : UICollectionView
    {
        protected DynamicCollectionView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (!Bounds.Size.Equals(IntrinsicContentSize))
            {
                InvalidateIntrinsicContentSize();
            }
        }

        public override CoreGraphics.CGSize IntrinsicContentSize
        {
            get
            {
                return ContentSize;
            }
        }
    }
}
