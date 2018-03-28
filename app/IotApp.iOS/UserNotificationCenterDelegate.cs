using System;
using IotApp.Azure;
using IotApp.Helpers;
using UserNotifications;

namespace IotApp.iOS
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        private readonly IAzureIoTHub _azureIoTHub;

        public UserNotificationCenterDelegate(IAzureIoTHub azureIoTHub)
        {
            _azureIoTHub = azureIoTHub;
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do something with the notification

            AppCenterHelper.Track("DidReceiveRemoteNotification - Active Notification");

            // Will also call '// App Active' in AppDelegate

            // Tell system to display the notification anyway or use
            // `None` to say we have handled the display locally.
            completionHandler(UNNotificationPresentationOptions.Alert);
        }

        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            // User Tapped
            if (response.IsDefaultAction)
            {
                var userInfo = response.Notification.Request.Content?.UserInfo;
                if (userInfo != null)
                {
                    AppCenterHelper.Track("DidReceiveNotificationResponse - User Tapped");
                }
            }
            completionHandler();
        }
    }
}
