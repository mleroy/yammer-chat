using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Parsers;
using Yammer.Chat.Core.Services;

namespace Yammer.Chat.Core.Repositories
{
    // TODO: find a way to get around this hack
    // The ThreadViewModel has a problem. It needs to indicate to the view that new messages came in, and notify its BottomScroller behavior that it should scroll all the way down
    // Subscribing to the Thread's Messages CollectionChanged sounds like it should work, but in practice there are UI bugs with the LongListSelector that make it show some items twice 
    // when scrolling while adding new items. 
    // The purpose of this hack is to have a single event notification when all new messages (from realtime) have been added to a thread, at which point it can tell the behavior to scroll...
    public interface INewMessagesListener
    {
        void OnNewMessagesArrived();
    }

    public interface IThreadRepository
    {
        ObservableCollection<Thread> Threads { get; }

        Task LoadThreads(long olderThanThreadId, int count);

        Task<Thread> GetThread(long id);

        Task RefreshThread(long id, int count);

        Task LoadThreadMessages(long threadId, long olderThanMessageId, int count);

        Task AddMessage(long threadId, string text, IEnumerable<Attachment> attachments = null);

        Task AddMessages(MessagesEnvelope messagesEnvelope, bool markThreadsAsUnread);

        Task MarkThreadAsRead(long threadId);

        Task LikeOrUnlikeMessage(long threadId, long messageId);

        void Clear();

        void ClearDraft();

        Task AddParticipants(long threadId, IEnumerable<User> newParticipants);

        Task LoadParticipantsDetails(long threadId, DetailsLevel detailsLevel = DetailsLevel.Full);

        Task<int> GetUnreadThreadCount(DateTime since);

        Task RefreshThreads(int count);

        INewMessagesListener NewMessagesListener { get; set; }
    }

    public class ThreadRepository : IThreadRepository
    {
        private readonly IMessagesService messagesService;
        private readonly IThreadParser threadParser;
        private readonly IMessageParser messageParser;
        private readonly IAttachmentParser attachmentParser;
        private readonly IParticipantParser participantParser;
        private readonly IUserRepository userRepository;

        public ObservableCollection<Thread> Threads { get; set; }

        private Thread draftThread;

        public INewMessagesListener NewMessagesListener { get; set; }

        public ThreadRepository(
            IMessagesService messagesService,
            IThreadParser threadParser,
            IMessageParser messageParser,
            IAttachmentParser attachmentParser,
            IParticipantParser participantParser,
            IUserRepository userRepository)
        {
            this.messagesService = messagesService;
            this.threadParser = threadParser;
            this.messageParser = messageParser;
            this.attachmentParser = attachmentParser;
            this.participantParser = participantParser;
            this.userRepository = userRepository;

            this.Threads = new ObservableCollection<Thread>();
        }

        public async Task LoadThreads(long olderThanThreadId, int count)
        {
            // TODO: this is not the right way to find older threads
            // Older (smaller) thread ids might be higher up. 
            // What needs to be compared is the timestamp of the lastmessage relative to the reference thread (indicated by olderThanThreadId)
            var cachedThreads = this.Threads.Where(t => t.Id < olderThanThreadId);
            var cachedThreadsCount = cachedThreads.Count();

            if (cachedThreadsCount < count)
            {
                var threadsNeededCount = count - cachedThreadsCount;

                var messagesEnvelope = await this.messagesService.GetChatThreads(olderThanThreadId, threadsNeededCount);
                var parsed = this.threadParser.Parse(messagesEnvelope);

                foreach (var thread in parsed)
                {
                    this.Threads.Add(thread);
                }
            }
        }

        public async Task RefreshThreads(int count)
        {
            var messagesEnvelope = await this.messagesService.GetChatThreads(long.MaxValue, count);
            var latestThreads = this.threadParser.Parse(messagesEnvelope);

            // Threads can have become out of view (past *count*), moved, and there can be new ones

            // Start by removing ones out of view
            for (var i = this.Threads.Count - 1; i >= 0; i--)
            {
                if (!latestThreads.Contains(this.Threads[i]))
                {
                    this.Threads.RemoveAt(i);
                }
            }

            for (var i = 0; i < latestThreads.Count(); i++)
            {
                var updatedThread = latestThreads[i];

                var indexOfUpdatedThreadInOldList = this.Threads.IndexOf(updatedThread);

                if (indexOfUpdatedThreadInOldList == -1)
                {
                    // The thread in the updated list did not exist before. Insert it.
                    this.Threads.Insert(i, updatedThread);
                }
                else if (indexOfUpdatedThreadInOldList != i)
                {
                    // The thread in the updated list existed before, but has moved
                    this.Threads.Remove(updatedThread);
                    this.Threads.Insert(i, updatedThread);

                    this.Threads[i].Merge(updatedThread);
                }
            }
        }

        public async Task<Thread> GetThread(long id)
        {
            if (id == 0)
            {
                if (this.draftThread == null)
                {
                    this.draftThread = await this.CreateDraft();
                }

                return this.draftThread;
            }

            var thread = this.Threads.FirstOrDefault(t => t.Id == id);

            if (thread == null)
            {
                var messagesEnvelope = await this.messagesService.GetThreadMessages(id, long.MaxValue, long.MinValue, 15);
                var parsed = this.threadParser.Parse(messagesEnvelope);

                thread = parsed.First();
            }

            return thread;
        }

        public async Task RefreshThread(long id, int count)
        {
            var thread = await this.GetThread(id);

            // We could use thread.LastMessage.Id as the newerThanMessageId parameter here
            // However then we would only get *new messages*, and unable to say if we'd be introducing a gap
            // By retrieving *count* messages, we have a high chance of retrieving at least one existing message and confirm the sequence
            var messagesEnvelope = await this.messagesService.GetThreadMessages(id, long.MaxValue, long.MinValue, count);
            var parsed = this.threadParser.ParseInThread(messagesEnvelope);

            thread.Merge(parsed);
        }

        private async Task<Thread> CreateDraft()
        {
            var thread = new Thread();
            var currentUser = await this.userRepository.GetCurrentUser();

            thread.AddParticipant(currentUser);

            return thread;
        }

        public async Task LoadThreadMessages(long threadId, long olderThanMessageId, int count)
        {
            var thread = await this.GetThread(threadId);

            var cachedMessages = thread.Messages.Where(m => m.Id < olderThanMessageId);
            var cachedMessagesCount = cachedMessages.Count();
            var allMessagesCached = thread.Messages.Count == thread.TotalMessages;

            if (cachedMessagesCount < count && !allMessagesCached)
            {
                var messagesNeededCount = count - cachedMessagesCount;

                var messagesEnvelope = await this.messagesService.GetThreadMessages(threadId, thread.Messages.First().Id, long.MinValue, messagesNeededCount);
                var messages = this.messageParser.Parse(messagesEnvelope);

                foreach (var message in messages.OrderByDescending(m => m.Id))
                {
                    thread.Messages.Insert(0, message);
                }
            }
        }

        public async Task AddMessage(long threadId, string text, IEnumerable<Attachment> attachments)
        {
            var thread = await this.GetThread(threadId);

            var attachmentDtos = this.attachmentParser.ToDto(attachments);

            var isReply = thread.Messages.Any();

            MessagesEnvelope messagesEnvelope = null;

            if (isReply)
            {
                messagesEnvelope = await this.messagesService.SendReply(thread.Id, text, attachmentDtos);
            }
            else
            {
                var participantsDtos = this.participantParser.ToDto(thread.Participants);
                messagesEnvelope = await this.messagesService.SendNewMessage(participantsDtos, text, attachmentDtos);
            }

            var messages = this.messageParser.Parse(messagesEnvelope);
            var message = messages.FirstOrDefault();

            thread.AddMessage(message);

            if (thread.IsDraft)
            {
                thread.Id = message.Id;

                this.ClearDraft();
            }

            this.BumpThread(thread);
        }

        public async Task AddMessages(MessagesEnvelope messagesEnvelope, bool markThreadsAsUnread)
        {
            var messages = this.messageParser.Parse(messagesEnvelope);

            foreach (var message in messages)
            {
                var thread = await this.GetThread(message.ThreadId);

                thread.AddMessage(message);

                if (!message.IsFromCurrentUser)
                {
                    thread.IsUnread |= markThreadsAsUnread;
                }

                if (thread.TotalMessages < thread.Messages.Count)
                {
                    // TODO: Seen this once...figure out why it happens
                    System.Diagnostics.Debugger.Break();
                }

                this.BumpThread(thread);
            }

            if (this.NewMessagesListener != null)
            {
                this.NewMessagesListener.OnNewMessagesArrived();
            }
        }

        private void BumpThread(Thread thread)
        {
            if (this.Threads.Contains(thread))
            {
                this.Threads.Remove(thread);
            }

            this.Threads.Insert(0, thread);
        }

        public async Task MarkThreadAsRead(long threadId)
        {
            var thread = await this.GetThread(threadId);

            await this.messagesService.SetLastSeenThreadMessage(thread.Id, thread.Messages.Last().Id);

            thread.IsUnread = false;
        }

        public void Clear()
        {
            this.Threads.Clear();
        }

        public void ClearDraft()
        {
            this.draftThread = null;
        }

        public async Task LikeOrUnlikeMessage(long threadId, long messageId)
        {
            var thread = await this.GetThread(threadId);
            var message = thread.Messages.First(x => x.Id == messageId);

            var currentUser = await this.userRepository.GetCurrentUser();

            if (message.IsLikedByCurrentUser)
            {
                await this.messagesService.UnlikeMessage(message.Id);
                message.UnlikedBy(currentUser);
                message.IsLikedByCurrentUser = false;
            }
            else
            {
                await this.messagesService.LikeMessage(message.Id);
                message.LikedBy(currentUser);
                message.IsLikedByCurrentUser = true;
            }
        }

        public async Task AddParticipants(long threadId, IEnumerable<User> users)
        {
            var thread = await this.GetThread(threadId);

            foreach (var user in users)
            {
                thread.AddParticipant(user);
            }

            var participantsDtos = this.participantParser.ToDto(users);

            if (!thread.IsDraft)
            {
                await this.messagesService.AddParticipants(threadId, participantsDtos);
            }
        }

        public async Task LoadParticipantsDetails(long threadId, DetailsLevel detailsLevel)
        {
            var thread = await this.GetThread(threadId);
            var currentUser = await this.userRepository.GetCurrentUser();

            foreach (var user in thread.Participants.Where(x => !x.Equals(currentUser) && x.AvailableDetails < detailsLevel))
            {
                var fullUser = await this.userRepository.GetUser(user.Id, detailsLevel);
                user.Merge(fullUser);
            }

            // Notify 'Participants' property listeners that collection has changed
            // This is not pretty; consumers should listen to the CollectionChanged event (via an ItemsControl control for example) and bind to each User's properties,
            // so that refreshing the Participants property should not be necessary.
            thread.NotifyOfPropertyChange(() => thread.Participants);
        }

        public async Task<int> GetUnreadThreadCount(DateTime since)
        {
            // Since an unread count doesn't seem to be returned by the API, we'll query pages of threads for unread threads.
            // We go back until the current page is older than the timeframe we're interested in (typically the time since we've last opened the app)
            var oldestThreadId = long.MaxValue;
            var pastTimestamp = false;

            // Limit how far back we go in case we never reach the boundary of the timeframe
            var pages = 0;

            // As high as the API supports is OK since this is used in contexts where performance isn't critical for UX (ex.: background agent)
            var countPerPage = 20;

            while (!pastTimestamp && pages++ < 5)
            {
                await this.LoadThreads(oldestThreadId, countPerPage);

                pastTimestamp = this.Threads.Last().LastMessage.Timestamp < since;
                oldestThreadId = this.Threads.Last().Id;
            }

            var count = this.Threads.Where(x =>
                x.IsUnread
                && x.LastMessage.Timestamp >= since).Count();

            return count;
        }
    }
}
