using Newtonsoft.Json;

namespace EasyNow.ApiClient.Getui
{
    public class PushSettings
    {
        [JsonProperty("ttl")]
        public int Ttl { get; set; }
    }
}