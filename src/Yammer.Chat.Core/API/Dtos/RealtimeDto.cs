using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API.Dtos
{
    public class RealtimeDto
    {
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}
