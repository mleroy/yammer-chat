using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Test;
using Yammer.Chat.ViewModels;

namespace Yammer.Chat.ViewModels.Test
{
    [TestClass]
    public class SettingsViewModelTests : TestBase
    {
        private Mock<INavigator> navigator;
        private Mock<IIdentityStore> identityStore;
        private Mock<IUserRepository> userRepository;
        private Mock<IThreadRepository> threadRepository;
        private Mock<IRealtimeManager> realtimeManager;

        [TestInitialize]
        public void Init()
        {
            this.navigator = new Mock<INavigator>();
            this.identityStore = new Mock<IIdentityStore>();
            this.userRepository = new Mock<IUserRepository>();
            this.threadRepository = new Mock<IThreadRepository>();
            this.realtimeManager = new Mock<IRealtimeManager>();
        }

        [TestMethod]
        public void logout()
        {
            var viewModel = getViewModel();

            viewModel.Logout();

            this.identityStore.Verify(r => r.Logout(), Times.Once, "Logout should call the repository's logout");

            this.realtimeManager.Verify(x => x.Disconnect(), Times.Once, "Logout should disconnect realtime");
            // TODO: verify cache cleared

            this.navigator.Verify(n => n.Navigate<LoginViewModel>(), Times.Once, "Logout should navigate to logout");
        }

        [TestMethod]
        public void navigation_to_edit_profile()
        {
            var viewModel = getViewModel();

            viewModel.EditProfile();

            this.navigator.Verify(n => n.Navigate<EditProfileViewModel>(), Times.Once, "Edit profile should navigate to profile");
        }

        private SettingsViewModel getViewModel()
        {
            return new SettingsViewModel(this.userRepository.Object, this.identityStore.Object, this.navigator.Object, null, this.realtimeManager.Object, this.threadRepository.Object, null, null);
        }
    }
}
