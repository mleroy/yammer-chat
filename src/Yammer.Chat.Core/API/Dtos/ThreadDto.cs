using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API.Dtos
{
    public class ThreadDto
    {
        [JsonProperty("thread_extended")]
        public Dictionary<int, MessageDto[]> Messages { get; set; }
    }
}
