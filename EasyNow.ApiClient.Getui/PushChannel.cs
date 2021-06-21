using System.Collections.Generic;
using Newtonsoft.Json;

namespace EasyNow.ApiClient.Getui
{
    public class PushChannel
    {
        [JsonProperty("android")]
        public ChannelAndroid Android { get; set; }
    }

    public class ChannelAndroid
    {
        [JsonProperty("ups")]
        public Ups Ups { get; set; }  
    }

    public class Ups
    {
        [JsonProperty("transmission")]
        public string Transmission { get; set; }

        [JsonProperty("notification")]
        public UpsNotification Notification { get; set; }

        [JsonProperty("options")]
        public Dictionary<string, Dictionary<string, object>> Options { get; set; }
    }

    public class UpsNotification
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("click_type")]
        public string ClickType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("payload")]
        public string Payload { get; set; }
    }
}