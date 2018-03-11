using System;

using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class ChipCollectionHeaderView : UICollectionReusableView
    {
        public static readonly NSString Key = new NSString("ChipCollectionHeaderView");

        protected ChipCollectionHeaderView(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
