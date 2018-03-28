using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Appliance.Helpers;
using MediatR;
using Microsoft.Azure.Devices.Client;
using Serilog;

namespace Appliance.Notifications
{
    /// <summary>
    /// Send a rich push notification, via AWS SNS Platform Application, with a media attachment of
    /// the UBNT camera snapshot of camera ID.
    /// </summary>
    public class SendRichPushNotificationHandler : INotificationHandler<SendRichPushNotificationCommand>
    {
        private readonly DeviceClient _deviceClient;

        public SendRichPushNotificationHandler(DeviceClient deviceClient)
        {
            _deviceClient = deviceClient;
        }

        public async Task Handle(SendRichPushNotificationCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                Dictionary<string, object> data = null;

                if (!string.IsNullOrWhiteSpace(notification.CameraId))
                {
                    var fileName = await GetCameraSnapshotAndUploadToBlob(notification);

                    data = new Dictionary<string, object>
                    {
                        {"media-attachment", string.Format(Config.UbntVideoCameraSnapshotImageUrl, fileName)}
                    };
                }

                // TODO: Send rich push notification using your preferred method
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private async Task<string> GetCameraSnapshotAndUploadToBlob(SendRichPushNotificationCommand notification)
        {
            var fileName = $"{string.Format("{0:s}", DateTime.Now).Replace(":", "-")}-" +
                           KeyGenerator.GetUniqueKey(100) +
                           ".jpg";

            using (var httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) => true;
                using (var client = new HttpClient(httpClientHandler))
                {
                    using (var stream =
                        await client.GetStreamAsync(string.Format(Config.UbntVideoCameraSnapshotEndpoint,
                            notification.CameraId)))
                    {
                        await _deviceClient.UploadToBlobAsync(fileName, stream);
                    }
                }
            }

            return fileName;
        }
    }

    public class SendRichPushNotificationCommand : INotification
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string CameraId { get; set; }
    }
}
