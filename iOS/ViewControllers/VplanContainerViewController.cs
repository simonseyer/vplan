using System;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class VplanContainerViewController : UIViewController, IUIScrollViewDelegate
    {
        public VplanViewModel ViewModel { get; set; }

        static readonly nfloat ScrollViewMargin = 12;
        static readonly nfloat PageSpacing = 6;

        NSLayoutConstraint TrailingConstraint;

        public VplanContainerViewController() : base("VplanContainerViewController", null)
        {
        }

        public VplanContainerViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ScrollView.Delegate = this;
            ViewModel.Dates.PropertyChanged += Dates_PropertyChanged;
            ReloadData();
        }

        void Dates_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                ReloadData();
            });
        }

        [Export("scrollViewDidEndDecelerating:")]
        public void DecelerationEnded(UIScrollView scrollView)
        {
            PageControl.CurrentPage = (int)((ScrollView.ContentOffset.X + 0.5 * ScrollView.Frame.Size.Width)  / ScrollView.Frame.Size.Width);
        }

        void ReloadData() 
        {
            var count = ViewModel.Dates.Value.Count;
            var diff = count - ChildViewControllers.Length;

            if (diff > 0)
            {
                AddPages(diff);
            } 
            if (diff < 0)
            {
                if (PageControl.CurrentPage > ViewModel.Dates.Value.Count)
                {
                    ScrollView.ScrollRectToVisible(CoreGraphics.CGRect.FromLTRB(0, 0, 0, 0), true);
                }
                RemovePages(Math.Abs(diff));
            }

            PageControl.Pages = count;

            for (var i = 0; i < count; i++)
            {
                var viewController = ChildViewControllers[i] as VplanDayViewController;
                viewController.PresentationModel = ViewModel.Dates.Value[i];
            }
        }

        void AddPages(int number)
        {
            UIViewController precedingViewController = null;
            if (ChildViewControllers.Length > 0)
            {
                precedingViewController = ChildViewControllers[ChildViewControllers.Length - 1];
                TrailingConstraint.Active = false;
            }

            for (var i = 0; i < number; i++)
            {
                var newViewController = Storyboard.InstantiateViewController("VplanDayViewController");
                AddChildViewController(newViewController);

                newViewController.View.TranslatesAutoresizingMaskIntoConstraints = false;
                ScrollView.AddSubview(newViewController.View);

                if (precedingViewController != null)
                {
                    newViewController.View.LeadingAnchor.ConstraintEqualTo(precedingViewController.View.TrailingAnchor, PageSpacing).Active = true;
                }
                else
                {
                    newViewController.View.LeadingAnchor.ConstraintEqualTo(ScrollView.LeadingAnchor, ScrollViewMargin).Active = true;
                }

                newViewController.View.TopAnchor.ConstraintEqualTo(ScrollView.TopAnchor).Active = true;
                newViewController.View.BottomAnchor.ConstraintEqualTo(ScrollView.BottomAnchor).Active = true;
                newViewController.View.HeightAnchor.ConstraintEqualTo(ScrollView.HeightAnchor).Active = true;
                newViewController.View.WidthAnchor.ConstraintEqualTo(ScrollView.WidthAnchor, 1, -2 * ScrollViewMargin).Active = true;

                newViewController.DidMoveToParentViewController(this);
                precedingViewController = newViewController;
            }

            TrailingConstraint = precedingViewController.View.TrailingAnchor.ConstraintEqualTo(ScrollView.TrailingAnchor, -ScrollViewMargin);
            TrailingConstraint.Active = true;
        }

        void RemovePages(int number)
        {
            TrailingConstraint.Active = false;

            for (var i = 0; i < number; i++)
            {
                var viewController = ChildViewControllers[ChildViewControllers.Length - 1];
                viewController.WillMoveToParentViewController(null);
                viewController.View.RemoveFromSuperview();
                viewController.RemoveFromParentViewController();
            }

            if (ChildViewControllers.Length > 0)
            {
                var lastViewController = ChildViewControllers[ChildViewControllers.Length - 1];
                TrailingConstraint = lastViewController.View.TrailingAnchor.ConstraintEqualTo(ScrollView.TrailingAnchor);
                TrailingConstraint.Active = true;
            }
        }
    }
}

