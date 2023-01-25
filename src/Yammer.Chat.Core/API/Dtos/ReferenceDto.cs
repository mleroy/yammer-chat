using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API.Dtos.Converters;

namespace Yammer.Chat.Core.API.Dtos
{
    [JsonConverter(typeof(ReferenceConverter))]
    public class ReferenceDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class UserReferenceDto : ReferenceDto
    {
        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("mugshot_url_template")]
        public string MugshotTemplate { get; set; }
    }

    public class GroupReferenceDto : ReferenceDto
    {
        [JsonProperty("full_name")]
        public string FullName { get; set; }
    }

    public class ConversationReferenceDto : ReferenceDto
    {
        [JsonProperty("participating_names")]
        public IList<ParticipantDto> Participants { get; set; }
    }

    public class ThreadReferenceDto : ReferenceDto
    {
        [JsonProperty("stats")]
        public ThreadStatsDto Stats { get; set; }
    }
}
