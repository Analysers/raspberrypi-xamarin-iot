using System;
using Plugin.Settings;

namespace IotApp
{
    public static class Config
    {
        public static string Person { get; } = "Occupant";

        private static Plugin.Settings.Abstractions.ISettings AppSettings => CrossSettings.Current;
        public static DateTime LastActionTime
        {
            get => AppSettings.GetValueOrDefault(nameof(DateTime), DateTime.UtcNow);
            set => AppSettings.AddOrUpdateValue(nameof(DateTime), value);
        }

        public static bool DebugMode
        {
            get => AppSettings.GetValueOrDefault(nameof(DebugMode), true);
            set => AppSettings.AddOrUpdateValue(nameof(DebugMode), value);
        }

        public static string DeviceToken
        {
            get => AppSettings.GetValueOrDefault(nameof(DeviceToken), "");
            set => AppSettings.AddOrUpdateValue(nameof(DeviceToken), value);
        }
    }
}
