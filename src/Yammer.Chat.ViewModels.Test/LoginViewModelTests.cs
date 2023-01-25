using Caliburn.Micro;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Exceptions;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;
using Yammer.Chat.Core.Test;
using Yammer.Chat.ViewModels;

namespace Yammer.Chat.ViewModels.Test
{
    [TestClass]
    public class LoginViewModelTests : TestBase
    {
        private Mock<INavigator> navigator;
        private Mock<IIdentityStore> identityStore;
        private Mock<IProgressIndicator> progressIndicator;

        [TestInitialize]
        public void Init()
        {
            this.navigator = new Mock<INavigator>();
            this.identityStore = new Mock<IIdentityStore>();
            this.progressIndicator = new Mock<IProgressIndicator>();
        }

        [TestMethod]
        public async Task ValidLoginNavigatesToThreadsView()
        {
            var viewModel = getViewModel();
            viewModel.Email = "sample@email.com";
            viewModel.Password = "password";

            this.identityStore.Setup(r => r.LoginAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.Factory.StartNew(() => { }));

            await viewModel.LoginAsync();

            this.navigator.Verify(n => n.Navigate<ThreadsViewModel>(), Times.Once, "Login should navigate to threads view");
            this.navigator.Verify(n => n.RemoveBackEntry(), Times.Once, "Navigation to threads view should remove login from history");
        }

        [TestMethod]
        public async Task MissingEmailIsInvalid()
        {
            var viewModel = getViewModel();
            viewModel.Password = "password";

            await viewModel.LoginAsync();

            Assert.IsTrue(viewModel.IsFormInvalid);
            this.navigator.Verify(n => n.Navigate<ViewModelBase>(), Times.Never, "Login should not navigate when email is missing");
        }

        [TestMethod]
        public async Task MissingPasswordIsValid()
        {
            this.identityStore.Setup(r => r.LoginAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.Factory.StartNew(() => { }));

            var viewModel = getViewModel();
            viewModel.Email = "sample@email.com";

            await viewModel.LoginAsync();

            Assert.IsFalse(viewModel.IsFormInvalid);
            this.navigator.Verify(n => n.Navigate<ViewModelBase>(), Times.Once, "Login should navigate when password is missing but credentials are valid.");
        }

        [TestMethod]
        public void EmailValidationClearsPreviousState()
        {
            var viewModel = getViewModel();
            viewModel.Email = "sample@email.com";
            viewModel.InvalidText = "Old error";

            viewModel.ValidateEmail();

            Assert.AreEqual(string.Empty, viewModel.InvalidText, "Email validation should clear previous state");
        }

        [TestMethod]
        public async Task LoginShowsProgress()
        {
            var viewModel = getViewModel();

            await viewModel.LoginAsync();

            this.progressIndicator.Verify(p => p.Show(It.IsAny<string>()), Times.Once, "Login in should report progress");
        }

        [TestMethod]
        public async Task LoginReachesRepository()
        {
            var viewModel = getViewModel();
            viewModel.Email = "sample@email.com";
            viewModel.Password = "password";

            await viewModel.LoginAsync();

            this.identityStore.Verify(r => r.LoginAsync(
                It.Is<string>(user => user == viewModel.Email), 
                It.Is<string>(password => password == viewModel.Password)), 
                Times.Once, "Login should reach into user repository");
        }

        [TestMethod]
        public async Task InvalidCredentialsException()
        {
            var viewModel = getViewModel();
            viewModel.Email = "sample@email.com";
            viewModel.Password = "password";
            this.identityStore.Setup(u => u.LoginAsync(It.IsAny<string>(), It.IsAny<string>())).Throws<InvalidCredentialsException>();

            await viewModel.LoginAsync();

            Assert.IsTrue(viewModel.IsFormInvalid, "Login form should be in invalid state after exception");
            Assert.AreEqual(AppResources.InvalidCredentialsText, viewModel.InvalidText, "Incorrect error message for exception");
        }

        [TestMethod]
        public async Task SsoException()
        {
            var viewModel = getViewModel();
            viewModel.Email = "sample@email.com";
            viewModel.Password = "password";
            this.identityStore.Setup(u => u.LoginAsync(It.IsAny<string>(), It.IsAny<string>())).Throws<SsoNetworkException>();

            await viewModel.LoginAsync();

            Assert.AreEqual(string.Empty, viewModel.InvalidText, "No error message should be shown for SSO network");
            this.navigator.Verify(n => n.Navigate<SsoLoginViewModel, string>(vm => vm.Email, viewModel.Email, NavigationFlags.None), Times.Once, "SSO exception should navigate to sso login view with email parameter.");
        }

        [TestMethod]
        public async Task GenericException()
        {
            var viewModel = getViewModel();
            viewModel.Email = "sample@email.com";
            viewModel.Password = "password";
            this.identityStore.Setup(r => r.LoginAsync(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            await viewModel.LoginAsync();

            Assert.IsTrue(viewModel.IsFormInvalid, "Login form should be in invalid state after exception");
            Assert.AreEqual(AppResources.DefaultErrorText, viewModel.InvalidText, "Incorrect error message for exception");
        }

        [TestMethod]
        public void ActivationClearsHistory()
        {
            IActivate viewModel = getViewModel();

            viewModel.Activate();

            this.navigator.Verify(n => n.ClearHistory(), Times.Once, "Loading the Login view should clear the history stack");
        }

        private LoginViewModel getViewModel()
        {
            return new LoginViewModel(this.navigator.Object, this.progressIndicator.Object, this.identityStore.Object);
        }
    }
}
