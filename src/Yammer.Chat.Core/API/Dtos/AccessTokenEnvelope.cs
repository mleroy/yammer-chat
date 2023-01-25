using Newtonsoft.Json;
using System;

namespace Yammer.Chat.Core.API.Dtos
{
    public class AccessTokenEnvelope
    {
        [JsonProperty("token")]
        public String Token { get; set; }

        [JsonProperty("user_id")]
        public Int64 UserId { get; set; }

        [JsonProperty("network_id")]
        public Int64 NetworkId { get; set; }

        [JsonProperty("network_name")]
        public String NetworkName { get; set; }

        [JsonProperty("network_permalink")]
        public String NetworkPermalink { get; set; }
    }
}
