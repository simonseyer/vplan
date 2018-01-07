using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Foundation;
using UIKit;

namespace FLSVertretungsplan.iOS
{
    public static class TextComponentFormatter
    {
        public static NSAttributedString AttributedStringForTextComponents(ICollection<TextComponent> textComponents, bool usePrimaryColor)
        {
            var attributedString = new NSMutableAttributedString();
            var font = UIFont.SystemFontOfSize(15);
            var primaryTextColor = usePrimaryColor ? Color.PRIMARY_TEXT_COLOR.ToUIColor() : Color.SECONDARY_TEXT_COLOR.ToUIColor();
            var secondaryTextColor = Color.SECONDARY_TEXT_COLOR.ToUIColor();
            foreach (var component in textComponents)
            {
                if (component.PrimaryText != null)
                {
                    var translatedText = NSBundle.MainBundle.LocalizedString(component.PrimaryText, "");
                    attributedString.Append(new NSAttributedString(translatedText, foregroundColor: primaryTextColor, font: font));
                }
                else if (component.SecondaryText != null)
                {
                    var translatedText = NSBundle.MainBundle.LocalizedString(component.SecondaryText, "");
                    attributedString.Append(new NSAttributedString(translatedText, foregroundColor: secondaryTextColor, font: font));
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

                    attributedString.Append(NSAttributedString.FromAttachment(iconAttachment));
                    attributedString.Append(new NSAttributedString(" ", font));
                }
            }

            var paragraphStyle = new NSMutableParagraphStyle
            {
                ParagraphSpacing = 0.25F * font.LineHeight
            };
            attributedString.AddAttributes(new UIStringAttributes { ParagraphStyle = paragraphStyle },
                                           new NSRange(0, attributedString.Length - 1));

            return attributedString;
        }

        public static string PlainStringForTextComponents(ICollection<TextComponent> textComponents)
        {

            var stringBuilder = new StringBuilder();
            foreach (var component in textComponents)
            {
                if (component.PrimaryText != null)
                {
                    stringBuilder.Append(NSBundle.MainBundle.LocalizedString(component.PrimaryText, ""));
                }
                else if (component.SecondaryText != null)
                {
                    stringBuilder.Append(NSBundle.MainBundle.LocalizedString(component.SecondaryText, ""));
                }
            }
            return stringBuilder.ToString();
        }
    }
}
