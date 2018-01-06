using System;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public partial class ChangeViewController : UIViewController
    {
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

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ChangeView.DisableShadows();
            PreferredContentSize = View.Bounds.Size;
        }


        ChangeTableViewCell ChangeView
        {
            get
            {
                return View as ChangeTableViewCell;
            }
        }

        ChangePresentationModel Model;

        public void SetPresentationModel(ChangePresentationModel model)
        {
            Model = model;
            ChangeView.SchoolClassLabel.Text = model.ClassName;
            ChangeView.HoursLabel.Text = model.Day + ", " + model.Hours;
            ChangeView.ChangeLabel.Text = NSBundle.MainBundle.LocalizedString(model.Type, "");
            ChangeView.SchoolGradientView.Gradient = model.FillColor;
            ChangeView.OriginalLessonLabel.AttributedText = TextComponentFormatter.AttributedStringForTextComponents(model.OldLesson, true);
            ChangeView.ChangeDescriptionLabel.AttributedText = TextComponentFormatter.AttributedStringForTextComponents(model.Description, false);
        }

        public UIViewController Context;

        public override IUIPreviewActionItem[] PreviewActionItems
        {
            get
            {
                return new IUIPreviewActionItem[] {
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
            var text = TextComponentFormatter.PlainStringForTextComponents(Model.TextRepresentation);
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
                theEvent.Title = TextComponentFormatter.PlainStringForTextComponents(Model.EventTitle);
                theEvent.Notes = TextComponentFormatter.PlainStringForTextComponents(Model.EventText);
                theEvent.AllDay = true;
                theEvent.StartDate = Model.Date.ToNSDate();
                theEvent.EndDate = Model.Date.ToNSDate();
                theEvent.Calendar = store.DefaultCalendarForNewEvents;

                NSError saveError;
                store.SaveEvent(theEvent, EventKit.EKSpan.ThisEvent, out saveError);
                if (saveError != null)
                {
                    System.Diagnostics.Debug.WriteLine("Failed to save calendar event: " + saveError.LocalizedDescription);
                }
            });
        }
    }
}

