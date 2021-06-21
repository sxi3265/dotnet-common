using Newtonsoft.Json;

namespace EasyNow.ApiClient.Getui
{
    public class CidAudience
    {
        [JsonProperty("cid")]
        public string[] Cid { get; set; }
    }
}