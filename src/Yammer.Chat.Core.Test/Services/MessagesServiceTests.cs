using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yammer.Chat.Core.API;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Services;
using Yammer.Chat.Core.Test.API;

namespace Yammer.Chat.Core.Test.Services
{
    [TestClass]
    public class MessagesServiceTests : TestBase
    {
        private Mock<IApiService> apiService;

        [TestInitialize]
        public void Init()
        {
            this.apiService = new Mock<IApiService>();
        }

        [TestMethod]
        public async Task gets_threads_from_service()
        {
            var service = getService();

            var expectation = new MessagesEnvelope();

            this.apiService
                .Setup(svc => svc.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(Task.FromResult((IApiResponse)new MockApiResponse(expectation)));

            var actual = await service.GetChatThreads(0, 1);

            Assert.AreSame(expectation, actual, "Messages service should get messages from the API service");
        }

        [TestMethod]
        public async Task threads_parameters_passed_to_service()
        {
            var service = getService();

            this.apiService
                .Setup(svc => svc.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(Task.FromResult((IApiResponse)new MockApiResponse(null)));

            await service.GetChatThreads(123, 1);

            this.apiService
                .Verify(x => x.GetAsync(
                    It.IsAny<string>(),
                    It.Is<ICollection<KeyValuePair<string, string>>>(parameters =>
                        parameters.Contains(new KeyValuePair<string, string>("older_than", "123"))
                        && parameters.Contains(new KeyValuePair<string, string>("limit", "1")))),
                    Times.Once, "Parameters should be passed to service");
        }

        [TestMethod]
        public async Task older_than_parameter_not_passed_when_null_for_threads()
        {
            var service = getService();
            this.apiService
                .Setup(svc => svc.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(Task.FromResult((IApiResponse)new MockApiResponse(null)));

            await service.GetChatThreads(-1, 0);

            this.apiService.Verify(x => x.GetAsync(
                It.IsAny<string>(),
                It.Is<ICollection<KeyValuePair<string, string>>>(parameters => !parameters.Any(p => p.Key == "older_than"))),
                Times.Once, "A null older_than parameter should not be part of the query");
        }

        [TestMethod]
        public async Task thread_messages_parameters_passed_to_service()
        {
            var threadId = 1;
            var olderThan = 2;
            var count = 3;

            var service = getService();

            this.apiService
                .Setup(svc => svc.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(Task.FromResult((IApiResponse)new MockApiResponse(null)));

            await service.GetThreadMessages(threadId, olderThan, long.MinValue, count);

            this.apiService
                .Verify(x => x.GetAsync(
                    It.Is<string>(endpoint => endpoint.Contains(threadId.ToString())),
                    It.Is<ICollection<KeyValuePair<string, string>>>(parameters =>
                        parameters.Contains(new KeyValuePair<string, string>("older_than", olderThan.ToString()))
                        && parameters.Contains(new KeyValuePair<string, string>("limit", count.ToString())))),
                    Times.Once, "Parameters should be passed to service");
        }

        [TestMethod]
        public async Task older_than_parameter_not_passed_when_null_for_thread_messages()
        {
            var threadId = 1;

            var service = getService();
            this.apiService
                .Setup(svc => svc.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(Task.FromResult((IApiResponse)new MockApiResponse(null)));

            await service.GetThreadMessages(threadId, long.MaxValue, long.MinValue, 1);

            this.apiService.Verify(x => x.GetAsync(
                It.IsAny<string>(),
                It.Is<ICollection<KeyValuePair<string, string>>>(parameters => !parameters.Any(p => p.Key == "older_than"))),
                Times.Once, "A null older_than parameter should not be part of the query");
        }

        [TestMethod]
        public async Task reply_message_parameters()
        {
            var replyToId = 1;
            var text = "Hello";
            var attachmentId = 2;
            var attachments = new[] { new AttachmentDto { Id = attachmentId } };

            this.apiService
                .Setup(svc => svc.PostAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                    null))
                .Returns(Task.FromResult((IApiResponse)new MockApiResponse(null)));

            var service = getService();

            await service.SendReply(replyToId, text, attachments);

            this.apiService
                .Verify(x => x.PostAsync(
                    It.IsAny<string>(),
                    It.Is<ICollection<KeyValuePair<string, string>>>(parameters =>
                        parameters.Contains(new KeyValuePair<string, string>("replied_to_id", replyToId.ToString()))
                     && parameters.Contains(new KeyValuePair<string, string>("body", text))
                     && parameters.Contains(new KeyValuePair<string, string>("attached_objects[]", "uploaded_file:" + attachmentId.ToString()))),
                     null),
                    Times.Once, "Service parameters should be api service query parameters");
        }

        [TestMethod]
        public async Task send_message_parameters()
        {
            var text = "Hello";
            var attachmentId = 1;
            var participantId = 2;
            var attachments = new[] { new AttachmentDto { Id = attachmentId } };
            var participants = new[] { new ParticipantDto { Id = participantId } };

            this.apiService
                .Setup(svc => svc.PostAsync(
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                    null))
                .Returns(Task.FromResult((IApiResponse)new MockApiResponse(null)));

            var service = getService();

            await service.SendNewMessage(participants, text, attachments);

            this.apiService
                .Verify(x => x.PostAsync(
                    It.IsAny<string>(),
                    It.Is<ICollection<KeyValuePair<string, string>>>(parameters =>
                        parameters.Contains(new KeyValuePair<string, string>("direct_to_user_ids", participantId.ToString()))
                     && parameters.Contains(new KeyValuePair<string, string>("body", text))
                     && parameters.Contains(new KeyValuePair<string, string>("attached_objects[]", "uploaded_file:" + attachmentId.ToString()))),
                     null),
                    Times.Once, "Service parameters should be api service query parameters");
        }

        [TestMethod]
        public async Task mark_thread_as_seen()
        {
            var threadId = 1;
            var messageId = 2;

            this.apiService
               .Setup(svc => svc.PostAsync(
                   It.IsAny<string>(),
                   It.IsAny<IEnumerable<KeyValuePair<string, string>>>(),
                   null))
               .Returns(Task.FromResult((IApiResponse)new MockApiResponse(null)));

            var service = getService();

            await service.SetLastSeenThreadMessage(threadId, messageId);

            this.apiService
                .Verify(x => x.PostAsync(
                    "/api/v1/messages/last_seen_in_thread",
                    It.Is<ICollection<KeyValuePair<string, string>>>(parameters => 
                        parameters.Contains(new KeyValuePair<string, string>("thread_id", threadId.ToString()))
                        && parameters.Contains(new KeyValuePair<string, string>("message_id", messageId.ToString()))),
                    null),
                    Times.Once, "Marking a thread as seen should make a post request with message_id parameter");
        }

        [TestMethod]
        public async Task cant_send_new_message_without_participants()
        {
            var service = getService();

            var exceptionThrown = false;

            try
            {
                await service.SendNewMessage(new List<ParticipantDto>(), "This is my message");
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public async Task cant_send_reply_without_message_id()
        {
            var service = getService();

            var exceptionThrown = false;

            try
            {
                await service.SendReply(0, "This is my reply");
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        private IMessagesService getService()
        {
            return new MessagesService(this.apiService.Object);
        }
    }
}
