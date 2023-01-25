using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API.Dtos
{
    public class MessagesEnvelope
    {
        public MessagesEnvelope()
        {
            this.Messages = new MessageDto[0];
            this.References = new ReferenceDto[0];
            this.Threads = new Dictionary<long, MessageDto[]>();
        }

        [JsonProperty("messages")]
        public MessageDto[] Messages { get; set; }

        [JsonProperty("references")]
        public ReferenceDto[] References { get; set; }

        [JsonProperty("meta")]
        public MetaDto Meta { get; set; }

        [JsonProperty("threaded_extended")]
        public Dictionary<long, MessageDto[]> Threads { get; set; }
    }

    public class MetaDto
    {
        public MetaDto()
        {
            this.UnseenMessageCountByThread = new Dictionary<long, int>();
        }

        [JsonProperty("current_user_id")]
        public long CurrentUserId { get; set; }

        [JsonProperty("unseen_message_count_by_thread")]
        public Dictionary<long, int> UnseenMessageCountByThread { get; set; }

        [JsonProperty("realtime")]
        public RealtimeDto Realtime { get; set; }
    }
}
