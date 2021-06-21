using Newtonsoft.Json;

namespace EasyNow.ApiClient.Getui
{
    public class PushMessage
    {
        /// <summary>
        /// 手机端通知展示时间段，格式为毫秒时间戳段，两个时间的时间差必须大于10分钟，例如："1590547347000-1590633747000"
        /// </summary>
        [JsonProperty("duration")]
        public string Duration { get; set; }

        /// <summary>
        /// 通知消息内容，仅支持安卓系统，iOS系统不展示个推通知消息，与transmission、revoke三选一，都填写时报错
        /// </summary>
        [JsonProperty("notification")]
        public Notification Notification { get; set; }

        /// <summary>
        /// 纯透传消息内容，安卓和iOS均支持，与notification、revoke 三选一，都填写时报错，长度 ≤ 3072
        /// </summary>
        [JsonProperty("transmission")]
        public string Transmission { get; set; }

        /// <summary>
        /// 撤回消息时使用(仅支持撤回个推通道消息)，与notification、transmission三选一，都填写时报错(消息撤回请勿填写策略参数)
        /// </summary>
        [JsonProperty("revoke")]
        public Revoke Revoke { get; set; }
    }
}