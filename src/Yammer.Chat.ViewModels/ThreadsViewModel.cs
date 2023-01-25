using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.API;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;
using Yammer.Chat.Core.Services;

namespace Yammer.Chat.ViewModels
{
    public class ThreadsViewModel : ViewModelBase
    {
        private readonly INavigator navigator;
        private readonly IThreadRepository threadRepository;
        private readonly IUserRepository userRepository;
        private readonly IProgressIndicator progressIndicator;
        private readonly IHttpService httpService;
        private readonly IResumeManager resumeManager;

        private const int PageSize = 15;

        public IHttpService HttpService { get { return this.httpService; } }

        public ObservableCollection<Thread> Threads { get { return this.threadRepository.Threads; } }

        private bool isLoadingThreads;
        public bool IsLoadingThreads
        {
            get { return this.isLoadingThreads; }
            set { base.SetProperty(ref this.isLoadingThreads, value); }
        }

        private Exception loadingThreadsException;
        public Exception LoadingThreadsException
        {
            get { return this.loadingThreadsException; }
            set { base.SetProperty(ref this.loadingThreadsException, value); }
        }

        public ThreadsViewModel(
            IThreadRepository threadRepository,
            IUserRepository userRepository,
            INavigator navigator,
            IProgressIndicator progressIndicator,
            IHttpService httpService,
            IResumeManager resumeManager)
        {
            this.threadRepository = threadRepository;
            this.userRepository = userRepository;
            this.navigator = navigator;
            this.progressIndicator = progressIndicator;
            this.httpService = httpService;
            this.resumeManager = resumeManager;
        }

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            await LoadMore();
        }

        protected async override void OnActivate()
        {
            base.OnActivate();

            if (this.resumeManager.NeedsToRefreshThreads)
            {
                this.resumeManager.NeedsToRefreshThreads = false;

                using (this.progressIndicator.Show())
                {
                    await this.threadRepository.RefreshThreads(PageSize);
                }
            }
        }

        public async Task LoadMore()
        {
            if (this.progressIndicator.IsShowing())
            {
                return;
            }

            this.IsLoadingThreads = true;

            using (this.progressIndicator.Show(AppResources.LoadingThreadsText))
            {
                try
                {
                    long olderThanId = this.Threads.Count == 0 ? long.MaxValue : this.Threads.Last().Id;
                    await this.threadRepository.LoadThreads(olderThanId, PageSize);
                }
                catch (Exception e)
                {
                    this.LoadingThreadsException = e;
                }
            }

            this.IsLoadingThreads = false;
        }

        public void ComposeNewThread()
        {
            // This is where we clean-up a pre-existing draft that user didn't send
            this.threadRepository.ClearDraft();

            this.navigator.Navigate<ParticipantSelectionViewModel>();

            Analytics.Default.LogEvent("ComposeNewThread");
        }

        public void NavigateToSettings()
        {
            this.navigator.Navigate<SettingsViewModel>();
        }

        public void ViewThread(Thread thread)
        {
            // We'll be loading a new instance of a thread, so we can forget about the flag to refresh a thread before a deactivation
            this.resumeManager.NeedsToRefreshThread = false;

            this.navigator.Navigate<ThreadViewModel, long>(threadVm => threadVm.ThreadId, thread.Id);

            Analytics.Default.LogEvent("ViewThread");
        }
    }
}
