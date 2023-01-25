using Newtonsoft.Json;
using System;

namespace Yammer.Chat.Core.API.Dtos
{
    public class NetworkEnvelope
    {
        [JsonProperty("id")]
        public Int64 Id { get; set; }

        [JsonProperty("name")]
        public String Name { get; set; }
    }
}
