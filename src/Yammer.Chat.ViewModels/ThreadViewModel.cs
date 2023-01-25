using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.API;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.ViewModels
{
    public class ThreadViewModel : ViewModelBase, INewMessagesListener
    {
        private readonly IThreadRepository threadRepository;
        private readonly IFileRepository fileRepository;
        private readonly IProgressIndicator progressIndicator;
        private readonly IPhotoChooser photoChooser;
        private readonly INavigator navigator;
        private readonly IHttpService httpService;
        private readonly IResumeManager resumeManager;

        private const int PageSize = 15;

        /// <summary>
        /// Navigation parameters
        /// </summary>
        public long ThreadId { get; set; }

        public ThreadViewModel(
            IThreadRepository threadRepository,
            IFileRepository fileRepository,
            IProgressIndicator progressIndicator,
            IPhotoChooser photoChooser,
            INavigator navigator,
            IHttpService httpService,
            IResumeManager resumeManager)
        {
            this.threadRepository = threadRepository;
            this.fileRepository = fileRepository;
            this.progressIndicator = progressIndicator;
            this.photoChooser = photoChooser;
            this.navigator = navigator;
            this.httpService = httpService;
            this.resumeManager = resumeManager;
        }

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            this.Thread = await this.threadRepository.GetThread(this.ThreadId);

            if (this.Thread.IsUnread)
            {
                await this.threadRepository.MarkThreadAsRead(this.ThreadId);
            }

            this.Thread.Messages.CollectionChanged += (s, e) =>
            {
                // TODO: figure out why notifying in this event handler ends up double-showing the first message in a new thread.
                // This does not happen if we notify in the SendMessage method. Maybe a bug in LLS, solved in telerik control?
                // Seems like the BottomScroller trigger's ScrollTo method causes every item to be re-realized in all cases, and when there's only one item, the first message is duplicated.
                //if (this.Thread.Messages.Count > 1)
                //    base.NotifyOfPropertyChange(() => this.LastMessage);
            };
        }

        protected async override void OnActivate()
        {
            base.OnActivate();

            if (this.resumeManager.NeedsToRefreshThread)
            {
                this.resumeManager.NeedsToRefreshThread = false;

                using (this.progressIndicator.Show())
                {
                    await this.threadRepository.RefreshThread(this.ThreadId, PageSize);
                }

                base.NotifyOfPropertyChange(() => this.LastMessage);
            }

            // We need first names for header, which come in the Full details package
            // Execute in all cases of navigating to this page (new, back navigations) to support compose & add participants scenarios
            await this.threadRepository.LoadParticipantsDetails(this.ThreadId, DetailsLevel.Full);

            this.threadRepository.NewMessagesListener = this;
        }

        protected override void OnDeactivate(bool close)
        {
            // Realtime messages make the thread unread
            if (this.Thread.IsUnread)
            {
                // Don't await, leave view immediately
                this.threadRepository.MarkThreadAsRead(this.ThreadId);
            }

            this.threadRepository.NewMessagesListener = null;

            base.OnDeactivate(close);
        }

        public void ViewMessage(Message message)
        {
            this.SelectedMessage = message;

            Analytics.Default.LogEvent("ViewMessage");
        }

        public async Task LikeUnlikeMessage(Message message)
        {
            using (this.progressIndicator.Show())
            {
                await this.threadRepository.LikeOrUnlikeMessage(this.Thread.Id, message.Id);
            }

            Analytics.Default.LogEvent("LikeUnlikeMessage", new Dictionary<string, object> { { "likedByCurrentUser", message.IsLikedByCurrentUser } });
        }

        public async Task LoadMore()
        {
            if (this.Thread.Messages.Count == this.Thread.TotalMessages || this.progressIndicator.IsShowing())
                return;

            using (this.progressIndicator.Show(AppResources.LoadMessagesText))
            {
                await this.threadRepository.LoadThreadMessages(this.Thread.Id, this.Thread.Messages.First().Id, PageSize);
            }
        }

        public async Task SendMessage()
        {
            using (this.progressIndicator.Show(AppResources.LoadingSendMessageText))
            {
                this.IsSending = true;

                try
                {
                    await this.threadRepository.AddMessage(this.Thread.Id, this.MessageText, null);

                    this.MessageText = string.Empty;
                }
                catch (Exception) { }

                this.IsSending = false;

                Analytics.Default.LogEvent("SendMessage");
            }
        }

        public async Task SendPhoto()
        {
            var photo = await this.getPhoto();

            if (photo != null)
            {
                using (this.progressIndicator.Show(AppResources.LoadingSendImageText))
                {
                    this.IsSending = true;

                    try
                    {
                        var attachment = await this.fileRepository.UploadImage(photo);

                        await this.threadRepository.AddMessage(this.Thread.Id, null, new[] { attachment });
                    }
                    catch (Exception) { }

                    this.IsSending = false;

                    Analytics.Default.LogEvent("SendPhoto");
                }
            }
        }

        public void ViewConversationDetails()
        {
            this.navigator.Navigate<ConversationDetailsViewModel, long>(vm => vm.ThreadId, thread.Id);
        }

        private async Task<PhotoChooserResult> getPhoto()
        {
            try
            {
                return await this.photoChooser.GetPhoto();
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public void ViewProfile()
        {
            this.navigator.Navigate<ProfileViewModel, long>(vm => vm.UserId, this.SelectedMessage.Sender.Id);
        }

        public string MessageText
        {
            get { return this.messageText; }
            set
            {
                base.SetProperty(ref this.messageText, value);
                NotifyOfPropertyChange(() => HasMessageText);
                NotifyOfPropertyChange(() => CanSendMessage);
            }
        }
        private string messageText;

        public bool HasMessageText
        {
            get { return !string.IsNullOrEmpty(this.MessageText); }
        }

        public bool CanSendMessage
        {
            get { return !string.IsNullOrWhiteSpace(this.MessageText) && !this.IsSending; }
        }

        public bool CanSendPhoto
        {
            get { return !this.IsSending; }
        }

        public Thread Thread
        {
            get { return this.thread; }
            set { base.SetProperty(ref this.thread, value); }
        }
        private Thread thread;

        public IHttpService HttpService { get { return this.httpService; } }

        public bool IsSending
        {
            get { return isSending; }
            set
            {
                base.SetProperty(ref this.isSending, value);
                NotifyOfPropertyChange(() => CanSendMessage);
                NotifyOfPropertyChange(() => CanSendPhoto);
            }
        }
        private bool isSending;

        public Message LastMessage
        {
            get { return this.Thread == null ? null : this.Thread.Messages.Last(); }
        }

        public Message SelectedMessage
        {
            get { return this.selectedMessage; }
            set { base.SetProperty(ref this.selectedMessage, value); }
        }
        private Message selectedMessage;

        public void OnNewMessagesArrived()
        {
            this.NotifyOfPropertyChange(() => this.LastMessage);
        }
    }
}
