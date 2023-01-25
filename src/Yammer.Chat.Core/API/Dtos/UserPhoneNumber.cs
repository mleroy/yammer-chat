using Newtonsoft.Json;
using System;

namespace Yammer.Chat.Core.API.Dtos
{
    public class UserPhoneNumber
    {
        [JsonProperty("type")]
        public String Type { get; set; }

        [JsonProperty("number")]
        public String Number { get; set; }
    }
}
