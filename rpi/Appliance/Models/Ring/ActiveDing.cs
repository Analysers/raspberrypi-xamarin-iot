using Newtonsoft.Json;

namespace Appliance.Models.Ring
{
    public class ActiveDing
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("id_str")]
        public string IdStr { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("protocol")]
        public string Protocol { get; set; }

        [JsonProperty("doorbot_id")]
        public long DoorbotId { get; set; }

        [JsonProperty("doorbot_description")]
        public string DoorbotDescription { get; set; }

        [JsonProperty("device_kind")]
        public string DeviceKind { get; set; }

        [JsonProperty("motion")]
        public bool Motion { get; set; }

        [JsonProperty("snapshot_url")]
        public string SnapshotUrl { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("sip_server_ip")]
        public string SipServerIp { get; set; }

        [JsonProperty("sip_server_port")]
        public long SipServerPort { get; set; }

        [JsonProperty("sip_server_tls")]
        public bool SipServerTls { get; set; }

        [JsonProperty("sip_session_id")]
        public string SipSessionId { get; set; }

        [JsonProperty("sip_from")]
        public string SipFrom { get; set; }

        [JsonProperty("sip_to")]
        public string SipTo { get; set; }

        [JsonProperty("audio_jitter_buffer_ms")]
        public long AudioJitterBufferMs { get; set; }

        [JsonProperty("video_jitter_buffer_ms")]
        public long VideoJitterBufferMs { get; set; }

        [JsonProperty("sip_endpoints")]
        public object SipEndpoints { get; set; }

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("now")]
        public double Now { get; set; }

        [JsonProperty("optimization_level")]
        public long OptimizationLevel { get; set; }

        [JsonProperty("sip_token")]
        public string SipToken { get; set; }

        [JsonProperty("sip_ding_id")]
        public string SipDingId { get; set; }
    }
}
