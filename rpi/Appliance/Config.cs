using System;
using Appliance.Domain;
using Newtonsoft.Json;
using Easy.Common.Extensions;

namespace Appliance
{
    public static class Config
    {
        public static IArmedState ArmedState { get; private set; }

        public static void Initialize(IArmedState armedState)
        {
            ArmedState = armedState;
        }

        public static TimeSpan MomentaryButtonPressMilliseconds { get; } = 750.Milliseconds();
        public static TimeSpan TriggeredNotifyLength { get; } = 5.Seconds();
        public static TimeSpan TriggeredWarnLength { get; } = 5.Seconds();
        public static TimeSpan TriggeredAlarmLength { get; } = 20.Minutes();
        public static TimeSpan SirenWarning { get; } = 500.Milliseconds();
        public static TimeSpan SirenAlarm { get; } = 2.Minutes();
        public static TimeSpan StrobeAlarm { get; } = 20.Minutes();
        public static TimeSpan GarageDoorOperationDuration { get; } = 20.Seconds();

        public static TimeSpan OccupantsHomeArmTime { get; } = new TimeSpan(21, 30, 00); // 9:30pm
        public static TimeSpan OccupantsHomeDisarmTime { get; } = new TimeSpan(06, 00, 00); // 6:00am
        public static OccupantState Occupant { get; set; } = new OccupantState();
        public static RelayState RelayState { get; set; } = new RelayState();
        public static decimal Temperature { get; set; } = 0;
        public static decimal Humidity { get; set; } = 0;

        public static bool OccupantsHome => Occupant.AtHome;
        public static bool OccupantAtBedroom => Occupant.IsSleeping;
        public static bool GarageDoorOperated { get; set; } = false;

        public static string DeviceConnectionString => "azure_iot_hub_device_connection_string";
        public static string OpenWeatherAppId => "open_weather_app_id";
        public static string RingBasicAuth => "ring_basic_auth";
        public static string RingHardwareId => "ring_hardware_id";
        public static string UbntVideoCameraSnapshotEndpoint => "https://192.168.1.30:7443/api/2.0/snapshot/camera/{0}?apiKey=ubnt_video_api_key&force=true";
        public static string UbntVideoCameraSnapshotImageUrl = "https://MYBLOBSTORAGEACCOUNT.blob.core.windows.net/folder/{0}";

        public static JsonSerializerSettings JsonSettings => new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
