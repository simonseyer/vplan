using System;
using System.Collections.Specialized;

using Foundation;
using UIKit;
using FLSVertretungsplan.iOS.Views;
using System.Collections.ObjectModel;

namespace FLSVertretungsplan.iOS
{
    public partial class VplanViewController : UITableViewController
    {
        UIRefreshControl refreshControl;

        DatePresentationModel _PresentationModel;
        public DatePresentationModel PresentationModel
        {
            get
            {
                return _PresentationModel;
            }
            set
            {
                if (_PresentationModel != null)
                {
                    _PresentationModel.IsRefreshing.PropertyChanged -= IsRefreshing_PropertyChanged;
                }
                _PresentationModel = value;
                if (_PresentationModel != null)
                {
                    _PresentationModel.IsRefreshing.PropertyChanged += IsRefreshing_PropertyChanged;
                }
                ReloadData();
            }
        }

        ItemsDataSource DataSource = new ItemsDataSource();

        public VplanViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Setup UITableView.
            refreshControl = new UIRefreshControl();
            refreshControl.Layer.ZPosition = -1;
            refreshControl.ValueChanged += RefreshControl_ValueChanged;
            TableView.RefreshControl = refreshControl;
            TableView.Source = DataSource;

            TableView.ClipsToBounds = true;
            TableView.Layer.CornerRadius = 6;
            TableView.Layer.BorderWidth = 0.5F;
            TableView.Layer.BorderColor = UIColor.FromRGB(236, 237, 241).CGColor;
            TableView.ContentInset = new UIEdgeInsets(14, 0, 14, 0);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        void RefreshControl_ValueChanged(object sender, EventArgs e)
        {
            PresentationModel.LoadItemsCommand.Execute(null);
        }

        void IsRefreshing_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                if (PresentationModel.IsRefreshing.Value && !refreshControl.Refreshing)
                {
                    refreshControl.BeginRefreshing();
                }
                else if (!PresentationModel.IsRefreshing.Value)
                {
                    refreshControl.EndRefreshing();
                }
            });
        }

        void ReloadData()
        {
            InvokeOnMainThread(() =>
            {
                DataSource.Items = PresentationModel?.Items;
                TableView.ReloadData();
                var lastUpdateText = NSBundle.MainBundle.LocalizedString("vplan_last_update", ""); 
                LastUpdateLabel.Text = NSString.LocalizedFormat(lastUpdateText, PresentationModel?.LastUpdate);
            });
        }
    }

    class ItemsDataSource : UITableViewSource
    {
        static readonly NSString CELL_IDENTIFIER = new NSString("CHANGE_CELL");

        public Collection<ChangePresentationModel> Items;

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section) 
        {
            return Items?.Count ?? 0;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            ChangeCell cell = (ChangeCell)tableView.DequeueReusableCell(CELL_IDENTIFIER, indexPath);

            var change = Items[indexPath.Row];

            cell.ClassNameLabel.Text = change.ClassName;
            cell.HoursLabel.Text = change.Hours;
            cell.ChangeTypeLabel.Text = NSBundle.MainBundle.LocalizedString(change.Type, "");
            cell.SchoolView.Gradient = change.FillColor;

            var primaryTextColor = UIColor.FromRGB(23, 43, 76);
            var secondaryTextColor = UIColor.FromRGB(164, 174, 186);
            var font = UIFont.SystemFontOfSize(15);
            var paragraphStyle = new NSMutableParagraphStyle
            {
                ParagraphSpacing = 0.25F * font.LineHeight
            };

            var originalLessonText = new NSMutableAttributedString();
            foreach (var component in change.OldLesson)
            {
                if (component.PrimaryText != null)
                {
                    var translatedText = NSBundle.MainBundle.LocalizedString(component.PrimaryText, "");
                    originalLessonText.Append(new NSAttributedString(translatedText, foregroundColor: primaryTextColor, font: font));
                }
                else if (component.SecondaryText != null)
                {
                    var translatedText = NSBundle.MainBundle.LocalizedString(component.SecondaryText, "");
                    originalLessonText.Append(new NSAttributedString(translatedText, foregroundColor: secondaryTextColor, font: font));
                }
            }
            cell.OriginalLessonLabel.AttributedText = originalLessonText;

            var descriptionText = new NSMutableAttributedString();
            foreach (var component in change.Description)
            {
                if (component.PrimaryText != null)
                {
                    var translatedText = NSBundle.MainBundle.LocalizedString(component.PrimaryText, "");
                    descriptionText.Append(new NSAttributedString(translatedText, foregroundColor: secondaryTextColor, font: font));
                }
                else if (component.SecondaryText != null)
                {
                    var translatedText = NSBundle.MainBundle.LocalizedString(component.SecondaryText, "");
                    descriptionText.Append(new NSAttributedString(translatedText, foregroundColor: secondaryTextColor, font: font));
                }
                else if (component.IconIdentifier != TextComponent.Icon.None)
                {
                    string icon = "info";
                    switch (component.IconIdentifier)
                    {
                        case TextComponent.Icon.Info:
                            icon = "info";
                            break;
                        case TextComponent.Icon.Location:
                            icon = "location";
                            break;
                        case TextComponent.Icon.Person:
                            icon = "person";
                            break;
                        case TextComponent.Icon.Subject:
                            icon = "book";
                            break;
                    }

                    var iconImage = UIImage.FromBundle(icon);
                    var iconAttachment = new NSTextAttachment
                    {
                        Image = iconImage,
                        Bounds = new CoreGraphics.CGRect(new CoreGraphics.CGPoint(0, font.Descender), iconImage.Size)
                    };

                    descriptionText.Append(NSAttributedString.FromAttachment(iconAttachment));
                    descriptionText.Append(new NSAttributedString(" ", font: font));
                }
            }
            descriptionText.AddAttributes(new UIStringAttributes { ParagraphStyle = paragraphStyle }, 
                                          new NSRange(0, descriptionText.Length - 1));
            cell.ChangeDescriptionLabel.AttributedText = descriptionText;


            cell.SchoolView.Layer.CornerRadius = 5;
            cell.SchoolView.ClipsToBounds = true;

            //cell.ContentBackgroundView.ClipsToBounds = true;
            cell.ContentBackgroundView.Layer.CornerRadius = 8;
            cell.ContentBackgroundView.Layer.BorderWidth = 1;
            cell.ContentBackgroundView.Layer.BorderColor = UIColor.FromRGB(227, 228, 232).CGColor;
            cell.ContentBackgroundView.Layer.ShadowColor = UIColor.Black.CGColor;
            cell.ContentBackgroundView.Layer.ShadowOffset = new CoreGraphics.CGSize(0, 2);
            cell.ContentBackgroundView.Layer.ShadowRadius = 3;
            cell.ContentBackgroundView.Layer.ShadowOpacity = 0.06F;

            return cell;
        }
    }
}
