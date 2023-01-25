using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API.Dtos.Converters;

namespace Yammer.Chat.Core.API.Dtos
{
    public class MugshotDto
    {
        [JsonProperty("image_id")]
        public string Id { get; set; }
    }
}
