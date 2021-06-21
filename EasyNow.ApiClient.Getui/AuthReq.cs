using EasyNow.Utility.Extensions;
using Newtonsoft.Json;

namespace EasyNow.ApiClient.Getui
{
    public class AuthReq
    {
        [JsonProperty("appkey")]
        public string AppKey { get; set; }
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("sign")]
        public string Sign => $"{AppKey}{Timestamp}{MasterSecret}".ToSha256String();

        [JsonIgnore]
        public string MasterSecret { get; set; }
    }
}