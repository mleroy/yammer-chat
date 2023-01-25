using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.ViewModels
{
    public class ParticipantSelectionViewModel : ViewModelBase
    {
        private readonly IThreadRepository threadRepository;
        private readonly IUserRepository userRepository;

        private readonly INavigator navigator;
        private readonly IProgressIndicator progressIndicator;

        private List<User> SelectedUsers { get; set; }
        private User CurrentUser { get; set; }

        private CancellationTokenSource lastFetchCancellationTokenSource;

        /// <summary>
        /// Navigation parameters
        /// </summary>
        public long ThreadId { get; set; }

        public ParticipantSelectionViewModel(IThreadRepository threadRepository, IUserRepository userRepository, INavigator navigator, IProgressIndicator progressIndicator)
        {
            this.threadRepository = threadRepository;
            this.userRepository = userRepository;

            this.navigator = navigator;
            this.progressIndicator = progressIndicator;

            this.Users = new ObservableCollection<User>();
            this.SelectedUsers = new List<User>();
        }

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            this.CurrentUser = await this.userRepository.GetCurrentUser();

            this.Thread = await this.threadRepository.GetThread(this.ThreadId);

            this.SelectedUsers.AddRange(this.Thread.Participants);
            base.NotifyOfPropertyChange(() => this.IsComposeEnabled);

            await this.RefreshUsers();
        }

        public async Task RefreshUsers(string searchText = null)
        {
            // Cancel any previous in-flight fetch
            if (this.lastFetchCancellationTokenSource != null)
            {
                if (this.lastFetchCancellationTokenSource.Token.CanBeCanceled)
                {
                    this.lastFetchCancellationTokenSource.Cancel();
                }
            }

            using (this.progressIndicator.Show())
            {
                try
                {
                    this.lastFetchCancellationTokenSource = new CancellationTokenSource();

                    var users = await this.userRepository.GetTopColleagues(searchText, this.lastFetchCancellationTokenSource.Token);

                    this.Users.Clear();

                    foreach (var user in users)
                    {
                        if (!user.Equals(this.CurrentUser))
                        {
                            user.IsSelected = this.SelectedUsers.Contains(user);
                            this.Users.Add(user);
                        }
                    }
                }
                catch (OperationCanceledException) { }
            }
        }

        public void ToggleUserSelection(User user)
        {
            // Ignore toggle if selected user is a thread participant. 
            // There might be a better way of telling the user this one can't be unselected.
            if (this.Thread.Participants.Contains(user))
            {
                return;
            }

            if (this.SelectedUsers.Contains(user))
            {
                this.SelectedUsers.Remove(user);
            }
            else
            {
                this.SelectedUsers.Add(user);
            }

            user.IsSelected = !user.IsSelected;
            base.NotifyOfPropertyChange(() => this.IsComposeEnabled);
        }

        public async Task Compose()
        {
            var newParticipants = this.SelectedUsers.Except(this.Thread.Participants).ToList();

            if (newParticipants.Count > 0)
            {
                using (this.progressIndicator.Show(AppResources.LoadingAddingParticipantsToThread))
                {
                    await this.threadRepository.AddParticipants(this.Thread.Id, newParticipants);
                }
            }

            if (this.Thread.IsDraft)
            {
                NavigationFlags navigationFlags = NavigationFlags.None;

                // There is generally no use in seeing this page after navigating away from it. 
                // For example, after sending a message, pressing 'Back' should not show this.
                navigationFlags |= NavigationFlags.RemoveCurrentPageFromBackStack;

                this.navigator.Navigate<ThreadViewModel, long>(vm => vm.ThreadId, this.Thread.Id, navigationFlags);
            }
            else
            {
                // ** Go back to ConversationDetails instead of ThreadView per iOS behavior. Uncomment if desired behavior is to go back to ThreadView **
                // As we navigate back to a ThreadViewModel, the Thread Id might be different from when we originally navigated to the ThreadView.
                // This happens when a new message was sent, where the original id was 0, then set to some value after a message was sent.
                // We ignore this scenario for our lookup; always go back to the ThreadView if it exists.
                //navigationFlags |= NavigationFlags.IgnoreParametersForBackStackLookup;
                //this.navigator.Navigate<ThreadViewModel, long>(vm => vm.ThreadId, this.Thread.Id, navigationFlags);

                this.navigator.GoBack();
            }
        }

        public ObservableCollection<User> Users { get; set; }

        public Yammer.Chat.Core.Models.Thread Thread
        {
            get { return this.thread; }
            set
            {
                base.SetProperty(ref this.thread, value);

                NotifyOfPropertyChange(() => this.PageTitle);
            }
        }
        private Yammer.Chat.Core.Models.Thread thread;

        public string PageTitle
        {
            get
            {
                if (this.Thread == null || this.Thread.IsDraft)
                {
                    return AppResources.NewConversationPageHeaderText;
                }

                return AppResources.AddCoworkersPageHeaderText;
            }
        }

        public bool IsComposeEnabled
        {
            get
            {
                // A single participant always indicates the current user only
                return this.SelectedUsers.Count() > 1;
            }
        }
    }
}
