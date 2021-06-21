using Newtonsoft.Json;

namespace EasyNow.ApiClient.Getui
{
    public class PushReq
    {
        /// <summary>
        /// 请求唯一标识号，10-32位之间；如果重复，会导致消息丢失
        /// </summary>
        [JsonProperty("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 推送目标用户
        /// </summary>
        [JsonProperty("audience")]
        public CidAudience Audience { get; set; }

        /// <summary>
        /// 推送条件设置
        /// </summary>
        [JsonProperty("settings")]
        public PushSettings Settings { get; set; }

        /// <summary>
        /// 个推推送消息参数
        /// </summary>
        [JsonProperty("push_message")]
        public PushMessage PushMessage { get; set; }

        /// <summary>
        /// 厂商推送消息参数，包含ios消息参数，android厂商消息参数
        /// </summary>
        [JsonProperty("push_channel")]
        public PushChannel PushChannel { get; set; }
    }
}