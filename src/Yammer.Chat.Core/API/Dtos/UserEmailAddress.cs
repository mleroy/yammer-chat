using Newtonsoft.Json;
using System;

namespace Yammer.Chat.Core.API.Dtos
{
    public class UserEmailAddress
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("address")]
        public String Address { get; set; }
    }
}
