using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API.Dtos
{
    public class ThreadStatsDto
    {
        [JsonProperty("updates")]
        public int TotalMessages { get; set; }

        [JsonProperty("first_reply_id")]
        public long? FirstReplyId { get; set; }
    }
}
