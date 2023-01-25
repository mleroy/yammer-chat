using Caliburn.Micro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Test;
using Yammer.Chat.ViewModels;

namespace Yammer.Chat.ViewModels.Test
{
    [TestClass]
    public class ShellViewModelTests : TestBase
    {
        private Mock<INavigator> navigator;
        private Mock<IIdentityStore> identityStore;

        [TestInitialize]
        public void Init()
        {
            this.navigator = new Mock<INavigator>();
            this.identityStore = new Mock<IIdentityStore>();
        }

        [TestMethod]
        public void ActivationGoesToThreadsWhenToken()
        {
            var viewModel = getViewModel();
            this.identityStore.SetupGet(store => store.IsLoggedIn).Returns(true);

            viewModel.GoToLandingView();

            this.navigator.Verify(n => n.Navigate<ThreadsViewModel>(), Times.Once, "A user with a token should be directed to the threads view");
            this.navigator.Verify(n => n.RemoveBackEntry(), Times.Once, "Shell view model should remove itself from history stack");
        }

        [TestMethod]
        public void ActivationGoesToLoginWhenNoToken()
        {
            var viewModel = getViewModel();
            this.identityStore.SetupGet(store => store.IsLoggedIn).Returns(false);

            viewModel.GoToLandingView();

            this.navigator.Verify(n => n.Navigate<LoginViewModel>(), Times.Once, "A user with no token should be directed to the login view");
            this.navigator.Verify(n => n.RemoveBackEntry(), Times.Once, "Shell view model should remove itself from history stack");
        }

        private ShellViewModel getViewModel()
        {
            return new ShellViewModel(this.navigator.Object, this.identityStore.Object);
        }
    }
}
