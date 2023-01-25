using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API.Dtos.Converters;

namespace Yammer.Chat.Core.API.Dtos
{
    [JsonConverter(typeof(AttachmentConverter))]
    public class AttachmentDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ImageAttachmentDto : AttachmentDto
    {
        [JsonProperty("preview_url")]
        public Uri Preview { get; set; }

        [JsonProperty("large_preview_url")]
        public Uri LargePreview { get; set; }
    }
}
