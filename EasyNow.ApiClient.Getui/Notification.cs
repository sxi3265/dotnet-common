using Newtonsoft.Json;

namespace EasyNow.ApiClient.Getui
{
    public class Notification
    {
        /// <summary>
        /// 通知消息标题，长度 ≤ 50
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 通知消息内容，长度 ≤ 256
        /// </summary>
        [JsonProperty("body")]
        public string Body { get; set; }

        /// <summary>
        /// 长文本消息内容，通知消息+长文本样式，与big_image二选一，两个都填写时报错，长度 ≤ 512
        /// </summary>
        [JsonProperty("big_text")]
        public string BigText { get; set; }

        /// <summary>
        /// 大图的URL地址，通知消息+大图样式， 与big_text二选一，两个都填写时报错，长度 ≤ 1024
        /// </summary>
        [JsonProperty("big_image")]
        public string BigImage { get; set; }
    }
}