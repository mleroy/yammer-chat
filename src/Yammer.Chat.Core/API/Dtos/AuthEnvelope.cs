using Newtonsoft.Json;

namespace Yammer.Chat.Core.API.Dtos
{
    public class AuthEnvelope
    {
        [JsonProperty("user")]
        public UserDto User { get; set; }

        [JsonProperty("access_token")]
        public AccessTokenEnvelope AccessToken { get; set; }

        [JsonProperty("network")]
        public NetworkEnvelope Network { get; set; }
    }
}
