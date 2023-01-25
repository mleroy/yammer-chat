using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;

namespace Yammer.Chat.Core.Parsers
{
    public interface IThreadParser
    {
        Thread[] Parse(MessagesEnvelope messagesEnvelope);
        Thread Parse(MessageDto threadDto, Dictionary<long, MessageDto[]> threadsDto, Dictionary<ReferenceKey, ReferenceDto> references, MetaDto meta);

        Thread ParseInThread(MessagesEnvelope messagesEnvelope);
    }

    public class ThreadParser : IThreadParser
    {
        private readonly IMessageParser messageParser;
        private readonly IUserParser userParser;
        private readonly IUserRepository userRepository;
        private readonly IRealtimeRepository realtimeRepository;

        public ThreadParser(IMessageParser messageParser, IUserParser userParser, IUserRepository userRepository, IRealtimeRepository realtimeRepository)
        {
            this.messageParser = messageParser;
            this.userParser = userParser;
            this.userRepository = userRepository;
            this.realtimeRepository = realtimeRepository;
        }

        public Thread[] Parse(MessagesEnvelope messagesEnvelope)
        {
            if (messagesEnvelope == null)
            {
                return new Thread[0];
            }

            var references = messagesEnvelope.References.ToDictionary(dto => new ReferenceKey(dto.Type, dto.Id), dto => dto);

            this.StoreUsers(references);
            this.realtimeRepository.SetRealtimeInfo(messagesEnvelope.Meta.Realtime);

            return messagesEnvelope.Messages.Select(threadStarterDto => this.Parse(threadStarterDto, messagesEnvelope.Threads, references, messagesEnvelope.Meta)).ToArray();
        }

        public Thread ParseInThread(MessagesEnvelope messagesEnvelope)
        {
            var thread = new Thread();

            var references = messagesEnvelope.References.ToDictionary(dto => new ReferenceKey(dto.Type, dto.Id), dto => dto);

            this.StoreUsers(references);
            this.realtimeRepository.SetRealtimeInfo(messagesEnvelope.Meta.Realtime);

            thread.Messages = new ObservableCollection<Message>(
                messagesEnvelope.Messages
                .Select(messageDto => this.messageParser.Parse(messageDto, references, messagesEnvelope.Meta))
                .OrderBy(m => m.Id));

            if (thread.Messages.Count > 0)
            {
                var firstMessageDto = messagesEnvelope.Messages.First();

                thread.Id = firstMessageDto.ThreadId;
                thread.Participants = ParseParticipants(firstMessageDto.ConversationId, references);
            }

            MergeThreadReference(thread, references);

            return thread;
        }

        public Thread Parse(MessageDto threadDto, Dictionary<long, MessageDto[]> threadsDto, Dictionary<ReferenceKey, ReferenceDto> references, MetaDto meta)
        {
            var thread = new Thread
            {
                Id = threadDto.ThreadId,
                Participants = ParseParticipants(threadDto.ConversationId, references),
                IsUnread = meta.UnseenMessageCountByThread.ContainsKey(threadDto.Id) && meta.UnseenMessageCountByThread[threadDto.Id] > 0
            };

            MergeThreadReference(thread, references);
            MergeMessages(thread, threadDto, threadsDto, references, meta);

            return thread;
        }

        private void MergeMessages(Thread thread, MessageDto threadDto, Dictionary<long, MessageDto[]> threadsDto, Dictionary<ReferenceKey, ReferenceDto> references, MetaDto meta)
        {
            var hasReplies = threadsDto.ContainsKey(threadDto.ThreadId);
            var messagesToParse = new List<MessageDto>();

            // If there are missing replies, don't include the thread starter. We'll retrieve it later along with the missing replies.
            // Otherwise, a thread needs to track the thread starter separately from replies, and it complicates business logic.
            if (!hasReplies || (threadsDto[threadDto.ThreadId].Length + 1 == thread.TotalMessages))
            {
                messagesToParse.Add(threadDto);
            }

            if (hasReplies)
            {
                messagesToParse.AddRange(threadsDto[threadDto.ThreadId]);
            }

            thread.Messages = new ObservableCollection<Message>(
                messagesToParse
                .Select(m => this.messageParser.Parse(m, references, meta))
                .OrderBy(m => m.Id));
        }

        private static void MergeThreadReference(Thread thread, Dictionary<ReferenceKey, ReferenceDto> references)
        {
            var key = ReferenceKey.ForThread(thread.Id);

            if (!references.ContainsKey(key))
            {
                return;
            }

            var threadReferenceDto = references[key] as ThreadReferenceDto;

            thread.TotalMessages = threadReferenceDto.Stats.TotalMessages;
            thread.FirstReplyId = threadReferenceDto.Stats.FirstReplyId;
        }

        private void StoreUsers(Dictionary<ReferenceKey, ReferenceDto> references)
        {
            foreach (var userReferenceDto in references.Where(x => x.Value is UserReferenceDto).Select(x => x.Value).Cast<UserReferenceDto>())
            {
                var user = this.userParser.ToModel(userReferenceDto);
                this.userRepository.AddOrUpdateUser(user);
            }

            // Even though all users are already present in user references, conversation references also hold first names that we'll add to the store.
            foreach (var conversationReferenceDto in references.Where(x => x.Value is ConversationReferenceDto).Select(x => x.Value).Cast<ConversationReferenceDto>())
            {
                foreach (var participantDto in conversationReferenceDto.Participants)
                {
                    var user = this.userParser.ToModel(participantDto);
                    this.userRepository.AddOrUpdateUser(user);
                }
            }
        }

        private ObservableCollection<User> ParseParticipants(long conversationId, Dictionary<ReferenceKey, ReferenceDto> references)
        {
            var key = ReferenceKey.ForConversation(conversationId);

            if (!references.ContainsKey(key))
            {
                return new ObservableCollection<User>();
            }

            var conversationReferenceDto = references[key] as ConversationReferenceDto;

            // Using .Result of GetUser task to avoid refactoring code with async/await
            // In practice, it's fine to block as the user will always be in cache.
            return new ObservableCollection<User>(
                conversationReferenceDto.Participants
                .Select(p => this.userRepository.GetUser(p.Id).Result));
        }
    }
}
