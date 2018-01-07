using System;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using UserNotifications;

namespace FLSVertretungsplan.iOS
{
    public class BackgroundFetchHandler
    {
        public void Setup()
        {
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
        }

        public async Task<bool> Fetch()
        {
            var dataStore = ServiceLocator.Instance.Get<IVplanDataStore>();
            await dataStore.Load();
            var diff = await dataStore.Refresh();
            if (diff == null)
            {
                throw new Exception();
            }

            var notificationCenter = UNUserNotificationCenter.Current;
            var settings = await notificationCenter.GetNotificationSettingsAsync();
            var oldNotifications = await notificationCenter.GetPendingNotificationRequestsAsync();

            if (settings.AuthorizationStatus == UNAuthorizationStatus.Authorized &&
                oldNotifications.Length == 0 &&
                (diff.NewBookmarkedChanges.Any() || diff.NewNewSchoolClassBookmarks.Any()))
            {
                var content = new UNMutableNotificationContent
                {
                    Title = NSBundle.MainBundle.LocalizedString("new_plan_title", "")
                };
                if (diff.NewBookmarkedChanges.Any())
                {
                    if (diff.NewBookmarkedChanges.Count() == 1)
                    {
                        content.Body = NSBundle.MainBundle.LocalizedString("new_changes_singular", "");
                    }
                    else
                    {
                        var text = NSBundle.MainBundle.LocalizedString("new_changes_plural", "");
                        content.Body = NSString.LocalizedFormat(text, diff.NewBookmarkedChanges.Count());
                    }
                    content.Badge = diff.NewBookmarkedChanges.Count();
                }
                else
                {
                    if (diff.NewNewSchoolClassBookmarks.Count() == 1)
                    {
                        content.Body = NSBundle.MainBundle.LocalizedString("new_school_classes_singular", "");
                    }
                    else
                    {
                        var text = NSBundle.MainBundle.LocalizedString("new_school_classes_plural", "");
                        content.Body = NSString.LocalizedFormat(text, diff.NewNewSchoolClassBookmarks.Count());
                    }
                    content.Badge = diff.NewNewSchoolClassBookmarks.Count();
                }

                var request = UNNotificationRequest.FromIdentifier(dataStore.SchoolVplan.Value.LastUpdate.ToString(), content, null);
                await notificationCenter.AddNotificationRequestAsync(request);
            }

            return diff.Updated;
        }
    }
}
