using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API.Dtos
{
    public class RealtimeDataDto
    {
        [JsonProperty("data")]
        public MessagesEnvelope MessagesEnvelope { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
