using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly INavigator navigator;
        private readonly IUserRepository userRepository;
        private readonly IIdentityStore identityStore;
        private readonly IApplicationVersion applicationVersion;
        private readonly IProgressIndicator progressIndicator;
        private readonly IEmailer emailer;

        // Things to reach out to when logging out. This is because we don't have an app-wide event bus (to reach into repositories/single instance cache)
        private readonly IThreadRepository threadRepository;
        private readonly IRealtimeManager realtimeManager;

        public SettingsViewModel(IUserRepository userRepository, 
            IIdentityStore identityStore,
            INavigator navigator,
            IProgressIndicator progressIndicator, 
            IRealtimeManager realtimeManager,
            IThreadRepository threadRepository, 
            IApplicationVersion applicationVersion,
            IEmailer emailer)
        {
            this.userRepository = userRepository;
            this.identityStore = identityStore;
            this.navigator = navigator;
            this.progressIndicator = progressIndicator;
            this.applicationVersion = applicationVersion;
            this.emailer = emailer;

            this.realtimeManager = realtimeManager;
            this.threadRepository = threadRepository;
        }

        protected async override void OnActivate()
        {
            base.OnActivate();

            // If the profile was edited in the EditProfile view, the binding notifications were missed by this view
            // Calling NotifyOfPropertyChanged(() => CurrentUser) doesn't seem to do anything, maybe it knows that nothing has changed.
            // Setting it to null and re-fetching it (which should be 0 cost) seems to do the trick
            using (this.progressIndicator.Show())
            {
                this.CurrentUser = null;
                this.CurrentUser = await this.userRepository.GetCurrentUser(DetailsLevel.Full);
            }
        }

        public void EditProfile()
        {
            this.navigator.Navigate<EditProfileViewModel>();
        }

        public void SendFeedback()
        {
            this.emailer.Open("yammernow@outlook.com", AppResources.SendFeedbackSubjectText);
        }

        public void Logout()
        {
            this.identityStore.Logout();

            this.threadRepository.Clear();
            this.userRepository.Clear();
            this.realtimeManager.Disconnect();

            this.navigator.Navigate<OAuthLoginViewModel>();

            Analytics.Default.LogEvent("Logout");
        }

        public User CurrentUser
        {
            get { return this.currentUser; }
            set { base.SetProperty(ref this.currentUser, value); }
        }
        private User currentUser;

        public bool IsRealtimeConnected { get { return this.realtimeManager.IsClientConnected(); } }

        public string ApplicationVersion { get { return this.applicationVersion.VersionNumber; } }
    }
}
