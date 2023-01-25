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

namespace Yammer.Chat.Core.Test.Repositories
{
    [TestClass]
    public class UserRepositoryTests : TestBase
    {
        private Mock<IUserService> userService;
        private Mock<IIdentityStore> identityStore;

        private IUserParser userParser;

        [TestInitialize]
        public void Init()
        {
            this.userService = new Mock<IUserService>();
            this.identityStore = new Mock<IIdentityStore>();
            this.userParser = new UserParser();
        }

        [TestMethod]
        public async Task current_user_from_service()
        {
            var dto = new UserDto { Id = 1 };

            this.userService
                .Setup(x => x.GetCurrentUser())
                .Returns(Task.FromResult<UserDto>(dto));

            var user = await getRepository().GetCurrentUser();

            Assert.AreEqual(dto.Id, user.Id);
        }

        [TestMethod]
        public async Task update_user()
        {
            this.userService
                .Setup(x => x.GetCurrentUser())
                .Returns(Task.FromResult<UserDto>(new UserDto()));

            this.userService
                .Setup(x => x.UpdateUser(It.IsAny<User>()))
                .Returns(Task.FromResult(new object()));

            var repository = getRepository();

            await repository.UpdateCurrentUser("first", "last", "job", "summary", "work", "mobile");

            this.userService
                .Verify(x => x.UpdateUser(It.Is<User>(u =>
                    u.FirstName == "first"
                    && u.LastName == "last"
                    && u.FullName == "first last"
                    && u.JobTitle == "job"
                    && u.Summary == "summary"
                    && u.WorkPhone == "work"
                    && u.MobilePhone == "mobile"
                    )), Times.Once, "Repository should pass a User model to service");

            var user = await repository.GetCurrentUser();

            Assert.AreEqual("first", user.FirstName);
            Assert.AreEqual("last", user.LastName);
            Assert.AreEqual("first last", user.FullName);
            Assert.AreEqual("job", user.JobTitle);
            Assert.AreEqual("summary", user.Summary);
            Assert.AreEqual("work", user.WorkPhone);
            Assert.AreEqual("mobile", user.MobilePhone);
        }

        [TestMethod]
        public async Task get_current_user_with_same_level_of_details()
        {
            this.userService
                .Setup(x => x.GetCurrentUser())
                .Returns(Task.FromResult<UserDto>(new UserDto()));

            var repository = getRepository();

            repository.AddOrUpdateUser(new User { AvailableDetails = DetailsLevel.Minimal });

            await repository.GetCurrentUser(DetailsLevel.Minimal);

            this.userService
                .Verify(x => x.GetCurrentUser(), Times.Never, "Requesting a user with same level of details as what's available in the cache should not get user from service");
        }

        [TestMethod]
        public async Task get_current_user_with_more_details()
        {
            this.userService
                .Setup(x => x.GetCurrentUser())
                .Returns(Task.FromResult<UserDto>(new UserDto()));

            var repository = getRepository();

            repository.AddOrUpdateUser(new User { AvailableDetails = DetailsLevel.Minimal });

            await repository.GetCurrentUser(DetailsLevel.Full);

            this.userService
                .Verify(x => x.GetCurrentUser(), Times.Once, "Requesting a user with more details than what's available in the cache should get user from service");
        }

        [TestMethod]
        public async Task get_user_with_same_level_of_details()
        {
            this.userService
                .Setup(x => x.GetUser(It.IsAny<long>()))
                .Returns(Task.FromResult<UserDto>(new UserDto()));

            var repository = getRepository();

            repository.AddOrUpdateUser(new User { Id = 1, AvailableDetails = DetailsLevel.Minimal });

            await repository.GetUser(1, DetailsLevel.Minimal);

            this.userService
                .Verify(x => x.GetUser(It.IsAny<long>()), Times.Never, "Requesting a user with same level of details as what's available in the cache should not get user from service");
        }

        [TestMethod]
        public async Task get_user_with_more_details()
        {
            this.userService
                .Setup(x => x.GetUser(It.IsAny<long>()))
                .Returns(Task.FromResult<UserDto>(new UserDto()));

            var repository = getRepository();

            repository.AddOrUpdateUser(new User { Id = 1, AvailableDetails = DetailsLevel.Minimal });

            await repository.GetUser(1, DetailsLevel.Full);

            this.userService
                .Verify(x => x.GetUser(It.IsAny<long>()), Times.Once, "Requesting a user with more details than what's available in the cache should get user from service");
        }

        public IUserRepository getRepository()
        {
            return new UserRepository(this.userService.Object, this.userParser, this.identityStore.Object);
        }
    }
}
