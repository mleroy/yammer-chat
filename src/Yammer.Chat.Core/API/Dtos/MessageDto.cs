using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API.Dtos
{
    public class MessageDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("thread_id")]
        public long ThreadId { get; set; }

        [JsonProperty("conversation_id")]
        public long ConversationId { get; set; }

        [JsonProperty("sender_id")]
        public long SenderId { get; set; }

        [JsonProperty("body")]
        public MessageBody Body { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt;

        [JsonProperty("liked_by")]
        public LikedByDto LikedBy;

        [JsonProperty("attachments")]
        public AttachmentDto[] Attachments { get; set; }

        [JsonProperty("client_type")]
        public string ClientType { get; set; }

        public class MessageBody
        {
            [JsonProperty("plain")]
            public string Plain { get; set; }
        }

        public class LikedByDto
        {
            [JsonProperty("names")]
            public LikedByNameDto[] Users { get; set; }
        }

        public class LikedByNameDto
        {
            [JsonProperty("user_id")]
            public long UserId { get; set; }
        }
    }
}
