using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Parsers;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Services;

namespace Yammer.Chat.Core.Test.Parsers
{
    [TestClass]
    public class ThreadParserTests
    {
        private Mock<IMessageParser> messageParser;
        private Mock<IUserParser> userParser;
        private Mock<IUserRepository> userRepository;
        private Mock<IRealtimeRepository> realtimeRepository;

        [TestInitialize]
        public void TestInit()
        {
            this.messageParser = new Mock<IMessageParser>();
            this.userParser = new Mock<IUserParser>();
            this.userRepository = new Mock<IUserRepository>();
            this.realtimeRepository = new Mock<IRealtimeRepository>();
        }

        [TestMethod]
        public void parses_thread()
        {
            var threadStarter = new MessageDto { Id = 1, ConversationId = 2, ThreadId = 3 };
            var reply = new MessageDto { Id = 2 };
            var conversation = new ConversationReferenceDto { Id = threadStarter.ConversationId, Type = "conversation", Participants = new[] { new ParticipantDto() } };
            var thread = new ThreadReferenceDto { Id = threadStarter.ThreadId, Type = "thread", Stats = new ThreadStatsDto { FirstReplyId = 1, TotalMessages = 2 } };

            var messagesEnvelope = new MessagesEnvelope
            {
                Messages = new MessageDto[] { threadStarter },
                Threads = new Dictionary<long, MessageDto[]> { { threadStarter.ThreadId, new MessageDto[] { reply } } },
                References = new ReferenceDto[] { conversation, thread },
                Meta = new MetaDto()
            };

            this.messageParser
                .Setup(x => x.Parse(It.IsAny<MessageDto>(), It.IsAny<Dictionary<ReferenceKey, ReferenceDto>>(), It.IsAny<MetaDto>()))
                .Returns<MessageDto, Dictionary<ReferenceKey, ReferenceDto>, MetaDto>((message, references, meta) => { return new Message { Id = message.Id }; });

            this.userRepository
                .Setup(x => x.GetUser(It.IsAny<long>(), It.IsAny<DetailsLevel>()))
                .Returns(Task.FromResult(new User()));

            var threads = getParser().Parse(messagesEnvelope);
            Assert.AreEqual(1, threads.Count());

            var actual = threads.First();
            Assert.AreEqual(2, actual.Messages.Count());
            Assert.AreEqual(conversation.Participants.Count, actual.Participants.Count());
            Assert.AreEqual(thread.Stats.FirstReplyId, actual.FirstReplyId);
            Assert.AreEqual(thread.Stats.TotalMessages, actual.TotalMessages);
        }

        [TestMethod]
        public void parsed_messages_ordered_from_oldest_to_newest()
        {
            var messagesEnvelope = new MessagesEnvelope
            {
                Messages = new[] { new MessageDto { Id = 1, ThreadId = 1 } },
                Threads = new Dictionary<long, MessageDto[]>
                {
                    { 
                        1, new [] { 
                            new MessageDto { Id = 4 },    
                            new MessageDto { Id = 1 },    
                            new MessageDto { Id = 3 },    
                            new MessageDto { Id = 2 }    
                        }
                    }
                },
                Meta = new MetaDto()
            };

            this.messageParser
                .Setup(x => x.Parse(It.IsAny<MessageDto>(), It.IsAny<Dictionary<ReferenceKey, ReferenceDto>>(), It.IsAny<MetaDto>()))
                .Returns<MessageDto, Dictionary<ReferenceKey, ReferenceDto>, MetaDto>((message, references, meta) => { return new Message { Id = message.Id }; });

            var thread = getParser().Parse(messagesEnvelope).First();

            long previousId = 0;
            foreach (var message in thread.Messages)
            {
                Assert.IsTrue(message.Id > previousId, "Parsed messages should be ordered from oldest to newest");
                previousId = message.Id;
            }
        }

        private ThreadParser getParser()
        {
            return new ThreadParser(this.messageParser.Object, this.userParser.Object, this.userRepository.Object, this.realtimeRepository.Object);
        }
    }
}
