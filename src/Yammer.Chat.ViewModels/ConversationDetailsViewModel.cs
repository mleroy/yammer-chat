using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;

namespace Yammer.Chat.ViewModels
{
    public class ConversationDetailsViewModel : ViewModelBase
    {
        private readonly IThreadRepository threadRepository;
        private readonly IUserRepository userRepository;
        
        private readonly IProgressIndicator progressIndicator;
        private readonly INavigator navigator;

        public ConversationDetailsViewModel(IThreadRepository threadRepository, IUserRepository userRepository, IProgressIndicator progressIndicator, INavigator navigator)
        {
            this.threadRepository = threadRepository;
            this.userRepository = userRepository;
            this.progressIndicator = progressIndicator;
            this.navigator = navigator;
        }

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            
            this.Thread = await this.threadRepository.GetThread(this.ThreadId);

            using (this.progressIndicator.Show())
            {
                await UpdateUnknownPresences();
            }
        }

        public async Task UpdateUnknownPresences()
        {
            var usersWithoutPresence = this.Thread.Participants.Where(x => x.Presence == UserPresence.Unknown).ToList();

            if (usersWithoutPresence.Count > 0)
            {
                await this.userRepository.UpdatePresences(usersWithoutPresence.Select(x => x.Id));
            }
        }

        public void AddCoworkers()
        {
            this.navigator.Navigate<ParticipantSelectionViewModel, long>(vm => vm.ThreadId, this.Thread.Id);
        }

        public void ViewProfile(User user)
        {
            this.navigator.Navigate<ProfileViewModel, long>(vm => vm.UserId, user.Id);
        }

        public long ThreadId
        {
            get { return threadId; }
            set { base.SetProperty(ref threadId, value); }
        }
        private long threadId;

        public Thread Thread
        {
            get { return this.thread; }
            set { base.SetProperty(ref this.thread, value); }
        }
        private Thread thread;
    }
}
