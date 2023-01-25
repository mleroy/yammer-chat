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
    public class MessageParserTests
    {
        private Mock<IAttachmentParser> attachmentParser;
        private Mock<IUserParser> userParser;
        private Mock<IUserRepository> userRepository;

        [TestInitialize]
        public void TestInit()
        {
            this.attachmentParser = new Mock<IAttachmentParser>();
            this.userParser = new Mock<IUserParser>();
            this.userRepository = new Mock<IUserRepository>();
        }

        [TestMethod]
        public void ParsesBodyIntoParts()
        {
            this.userRepository
                .Setup(x => x.GetUser(It.IsAny<long>(), It.IsAny<DetailsLevel>()))
                .Returns(Task.FromResult(new User()));

            string text = "Hi Matt";
            var result = this.getParser().Parse(new MessageDto { Body = new MessageDto.MessageBody { Plain = text } }, new Dictionary<ReferenceKey, ReferenceDto>(), new MetaDto());
            Assert.AreEqual(text, result.Body);
            Assert.AreEqual(text, result.BodyParts[0].Text);
        }

        [TestMethod]
        public void ParsesUsersInBody()
        {
            this.userRepository
                .Setup(x => x.GetUser(It.IsAny<long>(), It.IsAny<DetailsLevel>()))
                .Returns(Task.FromResult(new User()));

            string text = "Hi [[user:1234]]";
            var reference = new UserReferenceDto { FullName = "Matt", Id = 1234, Type = "user" };
            var result = this.getParser().Parse(new MessageDto { Body = new MessageDto.MessageBody { Plain = text } }, new Dictionary<ReferenceKey, ReferenceDto>() { { ReferenceKey.ForUser(1234), reference } }, new MetaDto());

            Assert.AreEqual(result.Body, "Hi Matt");
            Assert.AreEqual(result.BodyParts[1].Text, "Matt");
        }

        [TestMethod]
        public void MessageParsing()
        {
            var userKey = ReferenceKey.ForUser(1);

            this.userRepository
                .Setup(x => x.GetUser(It.IsAny<long>(), It.IsAny<DetailsLevel>()))
                .Returns(Task.FromResult<User>(new User { Id = 1, FullName = "User A", MugshotTemplate = "http://www.google.com" }));

            var references = new Dictionary<ReferenceKey, ReferenceDto>() 
            { 
                {
                    userKey, new UserReferenceDto
                    {
                        Id= 1,
                        FullName = "User A",
                        Type = "user",
                        MugshotTemplate = "http://www.google.com"
                    }
                }
            };

            var messageDto = new MessageDto
            {
                SenderId = 1,
                Body = new MessageDto.MessageBody { Plain = string.Empty }
            };

            var message = this.getParser().Parse(messageDto, references, new MetaDto());

            Assert.IsNotNull(message, "Message parser should create an instance");
            Assert.IsNotNull(message.Sender, "Message parser should parser sender");
            Assert.AreEqual((references[userKey] as UserReferenceDto).FullName, message.Sender.FullName, "Message parser should parse message sender name");
            Assert.AreEqual((references[userKey] as UserReferenceDto).MugshotTemplate, message.Sender.MugshotTemplate, "Message parser should parse message sender mugshot");
        }

        private MessageParser getParser()
        {
            return new MessageParser(this.attachmentParser.Object, this.userParser.Object, this.userRepository.Object);
        }
    }
}
