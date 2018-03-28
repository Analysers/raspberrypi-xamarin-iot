using System;
using Foundation;
using UserNotifications;

namespace RichImageNotification
{
    // Example code from: https://github.com/ibm-mobile-push/xamarin/blob/master/plugins/Actions/Media%20Attachment/NotificationService.cs
    [Register("NotificationService")]
    public class NotificationService : UNNotificationServiceExtension
    {
        private class MediaDownloadDelegate : NSUrlSessionDownloadDelegate, INSUrlSessionDownloadDelegate
        {
            public Action<UNNotificationContent> ContentHandler { get; set; }
            public UNMutableNotificationContent BestAttemptContent { get; set; }

            public override void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
            {
                var identifier = new NSUuid().ToString();
                var attachmentUrl = new NSUrl(downloadTask.OriginalRequest.Url.LastPathComponent, NSFileManager.DefaultManager.GetTemporaryDirectory());

                NSFileManager.DefaultManager.Copy(location, attachmentUrl, out NSError error);
                if (error != null)
                {
                    Deliver();
                    return;
                }

                var attachment = UNNotificationAttachment.FromIdentifier(identifier, attachmentUrl, new NSDictionary(), out error);
                if (error != null)
                {
                    Deliver();
                    return;
                }

                BestAttemptContent.Attachments = new UNNotificationAttachment[] { attachment };
                Deliver();
            }

            public void Deliver()
            {
                ContentHandler(BestAttemptContent);
            }
        }

        private MediaDownloadDelegate DownloadDelegate;

        protected NotificationService(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
        {
            DownloadDelegate = new MediaDownloadDelegate
            {
                ContentHandler = contentHandler,
                BestAttemptContent = (UNMutableNotificationContent)request.Content.MutableCopy()
            };

            var url = new NSUrl(request.Content.UserInfo["media-attachment"].ToString());
            if (url != null)
            {
                InvokeOnMainThread(() =>
                {
                    var session = NSUrlSession.FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration, DownloadDelegate as INSUrlSessionDownloadDelegate, NSOperationQueue.MainQueue);
                    var task = session.CreateDownloadTask(NSUrlRequest.FromUrl(url));
                    task.Resume();
                });

                return;
            }

            DownloadDelegate.Deliver();
        }

        public override void TimeWillExpire()
        {
            // Called just before the extension will be terminated by the system.
            // Use this as an opportunity to deliver your "best attempt" at modified content, otherwise the original push payload will be used.

            DownloadDelegate.Deliver();
        }
    }
}
