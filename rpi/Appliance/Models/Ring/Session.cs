using Newtonsoft.Json;

namespace Appliance.Models.Ring
{
    public class Session
    {
        [JsonProperty("profile")]
        public Profile Profile { get; set; }
    }

    public class Profile
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("authentication_token")]
        public string AuthenticationToken { get; set; }

        [JsonProperty("features")]
        public Features Features { get; set; }

        [JsonProperty("hardware_id")]
        public string HardwareId { get; set; }

        [JsonProperty("explorer_program_terms")]
        public object ExplorerProgramTerms { get; set; }

        [JsonProperty("user_flow")]
        public string UserFlow { get; set; }
    }

    public class Features
    {
        [JsonProperty("remote_logging_format_storing")]
        public bool RemoteLoggingFormatStoring { get; set; }

        [JsonProperty("remote_logging_level")]
        public long RemoteLoggingLevel { get; set; }

        [JsonProperty("subscriptions_enabled")]
        public bool SubscriptionsEnabled { get; set; }

        [JsonProperty("stickupcam_setup_enabled")]
        public bool StickupcamSetupEnabled { get; set; }

        [JsonProperty("vod_enabled")]
        public bool VodEnabled { get; set; }

        [JsonProperty("nw_enabled")]
        public bool NwEnabled { get; set; }

        [JsonProperty("nw_v2_enabled")]
        public bool NwV2Enabled { get; set; }

        [JsonProperty("nw_user_activated")]
        public bool NwUserActivated { get; set; }

        [JsonProperty("ringplus_enabled")]
        public bool RingplusEnabled { get; set; }

        [JsonProperty("lpd_enabled")]
        public bool LpdEnabled { get; set; }

        [JsonProperty("reactive_snoozing_enabled")]
        public bool ReactiveSnoozingEnabled { get; set; }

        [JsonProperty("proactive_snoozing_enabled")]
        public bool ProactiveSnoozingEnabled { get; set; }

        [JsonProperty("owner_proactive_snoozing_enabled")]
        public bool OwnerProactiveSnoozingEnabled { get; set; }

        [JsonProperty("live_view_settings_enabled")]
        public bool LiveViewSettingsEnabled { get; set; }

        [JsonProperty("delete_all_settings_enabled")]
        public bool DeleteAllSettingsEnabled { get; set; }

        [JsonProperty("power_cable_enabled")]
        public bool PowerCableEnabled { get; set; }

        [JsonProperty("device_health_alerts_enabled")]
        public bool DeviceHealthAlertsEnabled { get; set; }

        [JsonProperty("chime_pro_enabled")]
        public bool ChimeProEnabled { get; set; }

        [JsonProperty("multiple_calls_enabled")]
        public bool MultipleCallsEnabled { get; set; }

        [JsonProperty("ujet_enabled")]
        public bool UjetEnabled { get; set; }

        [JsonProperty("multiple_delete_enabled")]
        public bool MultipleDeleteEnabled { get; set; }

        [JsonProperty("delete_all_enabled")]
        public bool DeleteAllEnabled { get; set; }

        [JsonProperty("lpd_motion_announcement_enabled")]
        public bool LpdMotionAnnouncementEnabled { get; set; }

        [JsonProperty("starred_events_enabled")]
        public bool StarredEventsEnabled { get; set; }

        [JsonProperty("chime_dnd_enabled")]
        public bool ChimeDndEnabled { get; set; }

        [JsonProperty("video_search_enabled")]
        public bool VideoSearchEnabled { get; set; }

        [JsonProperty("floodlight_cam_enabled")]
        public bool FloodlightCamEnabled { get; set; }

        [JsonProperty("nw_larger_area_enabled")]
        public bool NwLargerAreaEnabled { get; set; }

        [JsonProperty("ring_cam_battery_enabled")]
        public bool RingCamBatteryEnabled { get; set; }

        [JsonProperty("elite_cam_enabled")]
        public bool EliteCamEnabled { get; set; }

        [JsonProperty("doorbell_v2_enabled")]
        public bool DoorbellV2Enabled { get; set; }

        [JsonProperty("spotlight_battery_dashboard_controls_enabled")]
        public bool SpotlightBatteryDashboardControlsEnabled { get; set; }

        [JsonProperty("bypass_account_verification")]
        public bool BypassAccountVerification { get; set; }

        [JsonProperty("legacy_cvr_retention_enabled")]
        public bool LegacyCvrRetentionEnabled { get; set; }

        [JsonProperty("new_dashboard_enabled")]
        public bool NewDashboardEnabled { get; set; }

        [JsonProperty("ring_cam_enabled")]
        public bool RingCamEnabled { get; set; }

        [JsonProperty("ring_search_enabled")]
        public bool RingSearchEnabled { get; set; }

        [JsonProperty("ring_cam_mount_enabled")]
        public bool RingCamMountEnabled { get; set; }

        [JsonProperty("ring_alarm_enabled")]
        public bool RingAlarmEnabled { get; set; }

        [JsonProperty("in_app_call_notifications")]
        public bool InAppCallNotifications { get; set; }

        [JsonProperty("ring_cash_eligible_enabled")]
        public bool RingCashEligibleEnabled { get; set; }

        [JsonProperty("new_ring_player_enabled")]
        public bool NewRingPlayerEnabled { get; set; }
    }
}
