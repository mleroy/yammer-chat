using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using Yammer.Chat.ViewModels.Test;

namespace Yammer.Chat.ViewModels.Test
{
    [TestClass]
    public class ThreadViewModelTests : TestBase
    {
        private Mock<IThreadRepository> threadRepository;
        private Mock<IFileRepository> fileRepository;
        private Mock<IProgressIndicator> progressIndicator;
        private Mock<INavigator> navigator;
        private Mock<IPhotoChooser> photoChooser;

        [TestInitialize]
        public void Init()
        {
            this.threadRepository = new Mock<IThreadRepository>();
            this.fileRepository = new Mock<IFileRepository>();
            this.progressIndicator = new Mock<IProgressIndicator>();
            this.navigator = new Mock<INavigator>();
            this.photoChooser = new Mock<IPhotoChooser>();
        }

        [TestMethod]
        public async Task does_not_load_more_while_already_loading()
        {
            this.progressIndicator
                .Setup(x => x.IsShowing())
                .Returns(true);

            var viewModel = getViewModel();
            viewModel.Thread = new Thread();

            await viewModel.LoadMore();

            this.threadRepository
                .Verify(x => x.LoadThreadMessages(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>()), Times.Never, "Load more should be a no-op while already loading more");
        }

        [TestMethod]
        public async Task does_not_load_more_when_it_has_all_messages()
        {
            var viewModel = getViewModel();
            viewModel.Thread = new Thread { Messages = new ObservableCollection<Message>(new[] { new Message() }), TotalMessages = 1 };

            await viewModel.LoadMore();

            this.threadRepository
                .Verify(x => x.LoadThreadMessages(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>()), Times.Never, "Load more should be a no-op when it already has all messages");
        }

        [TestMethod]
        public async Task load_more_gets_older_messages()
        {
            var viewModel = getViewModel();
            viewModel.Thread = new Thread { Id = 1, Messages = new ObservableCollection<Message> { new Message { Id = 2 } }, TotalMessages = 2 };
            this.threadRepository
                .Setup(r => r.LoadThreadMessages(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<int>()))
                .Returns(Task.FromResult<IEnumerable<Message>>(new List<Message>()));

            await viewModel.LoadMore();

            this.threadRepository
                .Verify(r => r.LoadThreadMessages(
                    It.Is<long>(threadId => threadId == viewModel.Thread.Id),
                    It.Is<long>(olderThanId => olderThanId == viewModel.Thread.Messages.First().Id),
                    It.IsAny<int>()),
                Times.Once, "View model should load older messages");
        }

        [TestMethod]
        public void text_allows_posting()
        {
            var viewModel = getViewModel();

            viewModel.MessageText = "Text";

            Assert.IsTrue(viewModel.CanSendMessage, "Reply text should be sufficent to post a reply");
        }

        [TestMethod]
        public void empty_text_disallows_posting()
        {
            var viewModel = getViewModel();

            viewModel.MessageText = "";

            Assert.IsFalse(viewModel.CanSendMessage, "Empty reply text shouldn't allow posting");
        }

        [TestMethod]
        public void whitespace_disallows_posting()
        {
            var viewModel = getViewModel();

            viewModel.MessageText = " \r\n";

            Assert.IsFalse(viewModel.CanSendMessage, "Empty reply text shouldn't allow posting");
        }

        [TestMethod]
        public async Task attachment_added_from_photo_chooser()
        {
            var photoResult = new PhotoChooserResult();

            this.photoChooser
                .Setup(x => x.GetPhoto())
                .Returns(Task.FromResult<PhotoChooserResult>(photoResult));

            this.fileRepository
                .Setup(x => x.UploadImage(It.IsAny<PhotoChooserResult>()))
                .Returns(() => Task.FromResult(new Attachment()));

            var viewModel = getViewModel();
            viewModel.Thread = new Thread();

            await viewModel.SendPhoto();

            this.fileRepository.Verify(x => x.UploadImage(photoResult), Times.Once, "Adding a photo should upload it through repository");

            this.threadRepository
                .Verify(x => x.AddMessage(
                    It.IsAny<long>(),
                    null,
                    It.Is<IEnumerable<Attachment>>(attachments => attachments.Count() == 1)),
                    Times.Once(), "Sending an image send a message with empty body and an attachment to the repository");
        }

        [TestMethod]
        public async Task no_attachment_added_when_photo_chooser_cancelled()
        {
            this.photoChooser
                .Setup(x => x.GetPhoto())
                .Throws<TaskCanceledException>();

            var viewModel = getViewModel();
            viewModel.Thread = new Thread();

            await viewModel.SendPhoto();

            this.fileRepository.Verify(x => x.UploadImage(It.IsAny<PhotoChooserResult>()), Times.Never, "Canceling photo selection should not reach into repository");

            this.threadRepository
                .Verify(x => x.AddMessage(
                    It.IsAny<long>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Attachment>>()),
                    Times.Never, "Canceling photo selection should not reach into repository");
        }

        [TestMethod]
        public async Task message_sent_to_repository()
        {
            var threadId = 1;
            var messageText = "Hello";

            var viewModel = getViewModel();
            viewModel.Thread = new Thread { Id = threadId };
            viewModel.MessageText = messageText;

            await viewModel.SendMessage();

            this.threadRepository
                .Verify(x => x.AddMessage(
                    threadId,
                    messageText,
                    null),
                Times.Once(), "Sending a message should reach into repository");
        }

        [TestMethod]
        public async Task send_message_clears_text()
        {
            this.threadRepository.Setup(x => x.AddMessage(It.IsAny<long>(), It.IsAny<string>(), null)).Returns(Task.FromResult<object>(null));

            var viewModel = getViewModel();
            viewModel.Thread = new Thread();
            viewModel.MessageText = "Hello";

            await viewModel.SendMessage();

            Assert.AreEqual(string.Empty, viewModel.MessageText, "Message text should be cleared after sending message");
        }

        [TestMethod]
        public async Task send_message_does_not_clear_text_on_error()
        {
            var messageText = "Hello";

            this.threadRepository
                .Setup(x => x.AddMessage(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<IEnumerable<Attachment>>()))
                .Throws<Exception>();

            var viewModel = getViewModel();
            viewModel.MessageText = messageText;

            await viewModel.SendMessage();

            Assert.AreEqual(messageText, viewModel.MessageText, "Message text should be cleared after sending message");
        }

        [TestMethod]
        public void navigation_to_conversation_details()
        {
            var thread = new Thread();
            var viewModel = getViewModel();
            viewModel.Thread = thread;

            viewModel.ViewConversationDetails();

            this.navigator.Verify(n => n.Navigate<ConversationDetailsViewModel, long>(It.IsAny<Expression<Func<ConversationDetailsViewModel, long>>>(), It.Is<long>(id => id == thread.Id), NavigationFlags.None),
                Times.Once, "Should navigate to conversation details");
        }

        private ThreadViewModel getViewModel()
        {
            return new ThreadViewModel(this.threadRepository.Object, this.fileRepository.Object, this.progressIndicator.Object, this.photoChooser.Object, this.navigator.Object, null, null);
        }
    }
}
