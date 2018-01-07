using System;
using System.Diagnostics;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class ChangeViewController : UIViewController
    {
        ChangeViewModel _ViewModel;
        public ChangeViewModel ViewModel
        {
            get
            {
                return _ViewModel;
            }
            set
            {
                _ViewModel = value;
                UpdateView(value);
            }
        }
        public UIViewController Context;

        ChangeTableViewCell ChangeView
        {
            get
            {
                return View as ChangeTableViewCell;
            }
        }

        public override IUIPreviewActionItem[] PreviewActionItems
        {
            get
            {
                return new IUIPreviewActionItem[]
                {
                    UIPreviewAction.Create(NSBundle.MainBundle.LocalizedString("share_image", ""), UIPreviewActionStyle.Default, (action, previewViewController) =>
                    {
                        ShareImage();
                    }),
                    UIPreviewAction.Create(NSBundle.MainBundle.LocalizedString("share_text", ""), UIPreviewActionStyle.Default, (action, previewViewController) =>
                    {
                        ShareText();
                    }),
                    UIPreviewAction.Create(NSBundle.MainBundle.LocalizedString("share_calendar_entry", ""), UIPreviewActionStyle.Default, (action, previewViewController) =>
                    {
                        ShareCalendarEvent();
                    })
                };
            }
        }

        public ChangeViewController() : base("ChangeViewController", null)
        {
        }

        public ChangeViewController(IntPtr handle) : base(handle)
        {
        }

        public override void LoadView()
        {
            View = ChangeTableViewCell.Create();
        }

        void UpdateView(ChangeViewModel model)
        {
            ChangeView.DisableShadows();

            ChangeView.SchoolClassLabel.Text = model.ClassName;
            ChangeView.HoursLabel.Text = model.Day + ", " + model.Hours;
            ChangeView.ChangeLabel.Text = NSBundle.MainBundle.LocalizedString(model.Type, "");
            ChangeView.SchoolGradientView.Gradient = model.FillColor;
            ChangeView.OriginalLessonLabel.AttributedText = TextComponentFormatter.AttributedStringForTextComponents(model.OldLesson, true);
            ChangeView.ChangeDescriptionLabel.AttributedText = TextComponentFormatter.AttributedStringForTextComponents(model.Description, false);

            // Calculate the dynamic content size with the width restricted to the view's width and a boundless height. 
            // Set the priority for the width to required (so it doesn't get larger than the available space) the priority of the height to low
            PreferredContentSize = ChangeView.SystemLayoutSizeFittingSize(new CoreGraphics.CGSize(View.Bounds.Size.Width, nfloat.MaxValue), 1000, 100);
        }

        public void ShareImage()
        {
            UIGraphics.BeginImageContextWithOptions(View.Bounds.Size, false, UIScreen.MainScreen.Scale);
            ChangeView.DisableRoundedCorners();
            ChangeView.BackgroundColor = UIColor.White;
            View.DrawViewHierarchy(View.Bounds, true);
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            var activityViewController = new UIActivityViewController(new NSObject[] { image }, null);
            Context.PresentViewController(activityViewController, true, null);
        }

        public void ShareText()
        {
            var text = TextComponentFormatter.PlainStringForTextComponents(ViewModel.TextRepresentation);
            var activityViewController = new UIActivityViewController(new NSObject[] { new NSString(text) }, null);
            Context.PresentViewController(activityViewController, true, null);
        }

        public void ShareCalendarEvent()
        {
            var store = new EventKit.EKEventStore();
            store.RequestAccess(EventKit.EKEntityType.Event, (bool granted, NSError error) =>
            {
                if (!granted)
                {
                    return;
                }

                var theEvent = EventKit.EKEvent.FromStore(store);
                theEvent.Title = TextComponentFormatter.PlainStringForTextComponents(ViewModel.EventTitle);
                theEvent.Notes = TextComponentFormatter.PlainStringForTextComponents(ViewModel.EventText);
                theEvent.AllDay = true;
                theEvent.StartDate = ViewModel.Date.ToNSDate();
                theEvent.EndDate = ViewModel.Date.ToNSDate();
                theEvent.Calendar = store.DefaultCalendarForNewEvents;

                store.SaveEvent(theEvent, EventKit.EKSpan.ThisEvent, out NSError saveError);
                if (saveError != null)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to save calendar event: " + saveError.LocalizedDescription);
                }
            });
        }
    }
}

