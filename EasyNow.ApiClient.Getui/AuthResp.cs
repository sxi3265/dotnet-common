using Newtonsoft.Json;

namespace EasyNow.ApiClient.Getui
{
    public class AuthResp
    {
        [JsonProperty("expire_time")]
        public string ExpireTime { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}