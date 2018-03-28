using System.Collections.Generic;

namespace Appliance.Models
{
    public class PushNotificationPayload
    {
        public string Title { get; } = "";
        public string Body { get; } = "";
        public bool ContentAvailable { get; } = false;
        public bool MutableContent { get; } = false;
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();

        public PushNotificationPayload(string title, string body, bool contentAvailable, bool mutableContent, Dictionary<string, object> data)
        {
            Title = title;
            Body = body;
            ContentAvailable = contentAvailable;
            MutableContent = mutableContent;
            Data = data;
        }

        public PushNotificationPayload()
        {
            ContentAvailable = true;
        }
    }
}
