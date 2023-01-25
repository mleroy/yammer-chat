using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Services;
using Yammer.Chat.Core.Test;
using Yammer.Chat.ViewModels;

namespace Yammer.Chat.ViewModels.Test
{
    [TestClass]
    public class ThreadsViewModelTests : TestBase
    {
        private Mock<INavigator> navigator;
        private Mock<IThreadRepository> threadRepository;
        private Mock<IUserRepository> userRepository;
        private Mock<IProgressIndicator> progressIndicator;
        private Mock<IResumeManager> resumeManager;

        [TestInitialize]
        public void Init()
        {
            this.navigator = new Mock<INavigator>();
            this.threadRepository = new Mock<IThreadRepository>();
            this.userRepository = new Mock<IUserRepository>();
            this.progressIndicator = new Mock<IProgressIndicator>();
            this.resumeManager = new Mock<IResumeManager>();
        }

        [TestMethod]
        public async Task LoadsMessagesFromThreadRepository()
        {
            var expectedMessage = new Message { BodyParts = new[] { new MessagePart { Text = "Foo" } } };
            IEnumerable<Thread> threads = new List<Thread>() { new Thread { Messages = new ObservableCollection<Message>(new[] { expectedMessage }) } };

            this.threadRepository
                .Setup(r => r.LoadThreads(It.IsAny<long>(), It.IsAny<int>()))
                .Returns(Task.FromResult(threads));
            this.threadRepository.SetupGet(x => x.Threads).Returns(() => new ObservableCollection<Thread>(threads));

            var viewModel = getViewModel();

            await viewModel.LoadMore();

            Assert.AreEqual(1, viewModel.Threads.Count, "View model should load messages from thread repository");
            Assert.AreEqual(expectedMessage.Body, viewModel.Threads.First().Messages.First().Body, "View model should load messages from thread repository");
        }

        [TestMethod]
        public async Task LoadingThreadReportsGlobalProgress()
        {
            this.threadRepository.Setup(r => r.LoadThreads(It.IsAny<long>(), It.IsAny<int>())).Returns(Task.FromResult((IEnumerable<Thread>)null));

            await getViewModel().LoadMore();

            progressIndicator.Verify(p => p.Show(It.IsAny<string>()),
                Times.Once,
                "Loading messages should report progress");
        }

        [TestMethod]
        public async Task ExceptionInLoadingThreadsIsHandled()
        {
            var expectedException = new Exception();

            this.threadRepository.Setup(r => r.LoadThreads(It.IsAny<long>(), It.IsAny<int>())).Throws(expectedException);
            this.threadRepository.SetupGet(x => x.Threads).Returns(() => new ObservableCollection<Thread>());

            var viewModel = getViewModel();

            await viewModel.LoadMore();

            Assert.AreEqual(expectedException, viewModel.LoadingThreadsException, "View model should handle exceptions in loading threads");
        }

        [TestMethod]
        public void ViewThreadNavigation()
        {
            var thread = new Thread();
            var viewModel = getViewModel();

            viewModel.ViewThread(thread);

            this.navigator.Verify(n => n.Navigate<ThreadViewModel, long>(It.IsAny<Expression<Func<ThreadViewModel, long>>>(), It.Is<long>(id => id == thread.Id), NavigationFlags.None),
                Times.Once, "ViewThread should navigate to thread view and pass the thread id as a parameter.");
        }

        [TestMethod]
        public void NavigateToSettings()
        {
            var viewModel = getViewModel();

            viewModel.NavigateToSettings();

            this.navigator.Verify(n => n.Navigate<SettingsViewModel>(), Times.Once, "NavigateToSettings should navigate");
        }

        private ThreadsViewModel getViewModel()
        {
            return new ThreadsViewModel(this.threadRepository.Object, this.userRepository.Object, this.navigator.Object, this.progressIndicator.Object, null, this.resumeManager.Object);
        }
    }
}
