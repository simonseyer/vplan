using System;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class VplanContainerViewController : UIViewController, IUIScrollViewDelegate, IVplanTabContentViewController
    {
        public VplanViewModel ViewModel { get; set; }

        static readonly nfloat ScrollViewMargin = 12;
        static readonly nfloat PageSpacing = 6;

        NSLayoutConstraint TrailingConstraint;
        bool Visible;
        CancellationTokenSource HideOverlayCancellationToken;
        UINotificationFeedbackGenerator FeedbackGenerator;

        bool EmptyViewShown = false;

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
            ViewModel.LastRefreshFailed.PropertyChanged += LastRefreshFailed_PropertyChanged;
            ReloadData();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Visible = true;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            Visible = false;
        }

        void Dates_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvokeOnMainThread(ReloadData);
        }

        void LastRefreshFailed_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ViewModel.LastRefreshFailed.Value && Visible)
            {
                InvokeOnMainThread(() =>
                {
                    FeedbackGenerator = new UINotificationFeedbackGenerator();
                    FeedbackGenerator.NotificationOccurred(UINotificationFeedbackType.Error);

                    View.LayoutIfNeeded();
                    StatusViewHiddenConstraint.Active = false;
                    UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseOut, () => View.LayoutIfNeeded(), null);
                });

                HideOverlayCancellationToken?.Cancel();
                HideOverlayCancellationToken = new CancellationTokenSource();
                Task.Delay(3000, HideOverlayCancellationToken.Token).ContinueWith(t2 =>
                {
                    if (t2.IsCanceled)
                    {
                        return;
                    }
                    InvokeOnMainThread(() =>
                    {
                        View.LayoutIfNeeded();
                        StatusViewHiddenConstraint.Active = true;
                        UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveEaseIn, () => View.LayoutIfNeeded(), null);
                        FeedbackGenerator = null;
                    });
                });
            }
        }

        [Export("scrollViewDidEndDecelerating:")]
        public void DecelerationEnded(UIScrollView scrollView)
        {
            PageControl.CurrentPage = (int)((ScrollView.ContentOffset.X + 0.5 * ScrollView.Frame.Size.Width)  / ScrollView.Frame.Size.Width);
        }

        void ReloadData() 
        {
            if (EmptyViewShown)
            {
                RemovePages(1);
                EmptyViewShown = false;
            }

            var count = ViewModel.Dates.Value.Count;
            var diff = count - ChildViewControllers.Length;

            if (diff > 0)
            {
                AddPages(diff, "VplanDayViewController");
            } 
            if (diff < 0)
            {
                if (PageControl.CurrentPage > ViewModel.Dates.Value.Count)
                {
                    ScrollView.ScrollRectToVisible(CGRect.FromLTRB(0, 0, 0, 0), true);
                }
                RemovePages(Math.Abs(diff));
            }

            PageControl.Pages = count > 1 ? count : 0;

            if (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var viewController = ChildViewControllers[i] as VplanDayViewController;
                    viewController.ViewModel = ViewModel.Dates.Value[i];
                }
            }
            else
            {
                EmptyViewShown = true;
                AddPages(1, "EmptyVplanViewController");
                var viewController = ChildViewControllers[0] as EmptyVplanViewController;
                viewController.ViewModel = ViewModel;
            }
        }

        void AddPages(int number, string type)
        {
            UIViewController precedingViewController = null;
            if (ChildViewControllers.Length > 0)
            {
                precedingViewController = ChildViewControllers[ChildViewControllers.Length - 1];
                TrailingConstraint.Active = false;
            }

            for (var i = 0; i < number; i++)
            {
                var newViewController = Storyboard.InstantiateViewController(type);
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

        public void ResetContent()
        {
            ScrollView.SetContentOffset(CGPoint.Empty, true);
            PageControl.CurrentPage = 0;
        }
    }
}

