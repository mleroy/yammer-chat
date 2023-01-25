using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API;
using Yammer.Chat.Core.API.Dtos;

namespace Yammer.Chat.Core.Services
{
    public interface IMessagesService
    {
        Task<MessagesEnvelope> GetChatThreads(long olderThanId, int count);

        Task<MessagesEnvelope> GetThreadMessages(long threadId, long olderThanMessageId, long newerThanMessageId, int count);

        Task<MessagesEnvelope> SendNewMessage(IEnumerable<ParticipantDto> participants, string text, IEnumerable<AttachmentDto> attachments = null);

        Task<MessagesEnvelope> SendReply(long messageId, string text, IEnumerable<AttachmentDto> attachments = null);

        Task SetLastSeenThreadMessage(long threadId, long messageId);

        Task LikeMessage(long messageId);

        Task UnlikeMessage(long messageId);

        Task AddParticipants(long threadId, IEnumerable<ParticipantDto> participantsDtos);
    }

    public class MessagesService : IMessagesService
    {
        private readonly IApiService apiService;

        public MessagesService(IApiService apiService)
        {
            this.apiService = apiService;
        }

        async Task<MessagesEnvelope> IMessagesService.GetChatThreads(long olderThanId, int count)
        {
            var parameters = new Dictionary<string, string>()
            { 
            /* 
             * These seem unnecessary   
                { "update_last_seen_message_id", "false" },
                { "exclude_own_messages_from_unseen", "true"},
                { "inbox_supported_client", "true"},
                { "include_counts", "true"},
             * */
                { "threaded", "extended" },
                { "limit", count.ToString() },
                { "filter", "chat"}
            };

            if (olderThanId > -1 && olderThanId < long.MaxValue)
            {
                parameters.Add("older_than", olderThanId.ToString());
            }

            var response = await this.apiService.GetAsync("/api/v1/messages/private.json", parameters);
            return response.ToEntity<MessagesEnvelope>();
        }

        async Task<MessagesEnvelope> IMessagesService.GetThreadMessages(long threadId, long olderThanMessageId, long newerThanMessageId, int count)
        {
            var parameters = new Dictionary<string, string>()
            { 
                { "update_last_seen_message_id", "true" },
                { "exclude_own_messages_from_unseen", "true"},
                { "threaded", "false" },
                { "limit", count.ToString() },
                { "inbox_supported_client", "true"},
            };

            if (olderThanMessageId < long.MaxValue)
            {
                parameters.Add("older_than", olderThanMessageId.ToString());
            }

            if (newerThanMessageId > long.MinValue)
            {
                parameters.Add("newer_than", newerThanMessageId.ToString());
            }

            var endpoint = string.Format("/api/v1/messages/in_thread/{0}.json", threadId);

            var response = await this.apiService.GetAsync(endpoint, parameters);
            return response.ToEntity<MessagesEnvelope>();
        }
        Task<MessagesEnvelope> IMessagesService.SendNewMessage(IEnumerable<ParticipantDto> participants, string text, IEnumerable<AttachmentDto> attachments)
        {
            if (participants == null || !participants.Any())
            {
                throw new ArgumentException("Sending a new message requires participants");
            }

            return this.SendMessage(participants, -1, text, attachments);
        }

        Task<MessagesEnvelope> IMessagesService.SendReply(long messageId, string text, IEnumerable<AttachmentDto> attachments)
        {
            if (messageId <= 0)
            {
                throw new ArgumentException("Sending a reply requires a message id");
            }

            return this.SendMessage(null, messageId, text, attachments);
        }

        private async Task<MessagesEnvelope> SendMessage(IEnumerable<ParticipantDto> participants, long replyToId, string text, IEnumerable<AttachmentDto> attachments)
        {
            var parameters = new List<KeyValuePair<string, string>>()
            { 
               new KeyValuePair<string, string>("format", "json"),
               new KeyValuePair<string, string>("message_type", "chat")
            };

            if (!string.IsNullOrEmpty(text))
            {
                parameters.Add(new KeyValuePair<string, string>("body", text));
            }

            if (replyToId > -1)
            {
                parameters.Add(new KeyValuePair<string, string>("replied_to_id", replyToId.ToString()));
            }

            if (participants != null && participants.Any())
            {
                parameters.Add(new KeyValuePair<string, string>("direct_to_user_ids", string.Join(",", participants.Select(x => x.Id))));
            }

            if (attachments != null && attachments.Any())
            {
                foreach (var attachment in attachments)
                {
                    parameters.Add(new KeyValuePair<string, string>("attached_objects[]", string.Format("uploaded_file:{0}", attachment.Id)));
                }
            }

            var response = await this.apiService.PostAsync("/api/v1/messages", parameters);
            return response.ToEntity<MessagesEnvelope>();
        }

        public Task SetLastSeenThreadMessage(long threadId, long messageId)
        {
            var parameters = new Dictionary<string, string>
            {
                { "thread_id", threadId.ToString() },
                { "message_id", messageId.ToString() }
            };

            return this.apiService.PostAsync("/api/v1/messages/last_seen_in_thread", parameters);
        }

        public Task LikeMessage(long messageId)
        {
            var parameters = new Dictionary<string, string>
            {
                { "message_id", messageId.ToString() }
            };

            return this.apiService.PostAsync("/api/v1/messages/liked_by/current.json", parameters);
        }

        public Task UnlikeMessage(long messageId)
        {
            var parameters = new Dictionary<string, string>
            {
                { "message_id", messageId.ToString() }
            };

            return this.apiService.DeleteAsync("/api/v1/messages/liked_by/current.json", parameters);
        }

        public Task AddParticipants(long threadId, IEnumerable<ParticipantDto> participants)
        {
            var parameters = new Dictionary<string, string>
            {
                { "include_user_ids", string.Join(",", participants.Select(x => x.Id)) }
            };

            var endpoint = string.Format("/api/v1/threads/{0}.json", threadId);

            return this.apiService.PutAsync(endpoint, parameters);
        }
    }
}
