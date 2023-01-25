using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Services;
using Yammer.Chat.Core.Test.Mocks;

namespace Yammer.Chat.Core.Test.Repositories
{
    [TestClass]
    public class IdentityStoreTests : TestBase
    {
        private Mock<IUserService> userService;
        private Mock<ITokenStore> tokenStore;
        private ISettings settings;

        [TestInitialize]
        public void Init()
        {
            this.userService = new Mock<IUserService>();
            this.tokenStore = new Mock<ITokenStore>();

            this.settings = new SettingsMock();
        }

        [TestMethod]
        public async Task login_saves_token_and_user_id()
        {
            var authEnvelope = new AuthEnvelope() { AccessToken = new AccessTokenEnvelope { Token = "token" }, User = new UserDto { Id = 1 } };
            this.userService.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(authEnvelope));
            var repository = getRepository();

            await repository.LoginAsync("", "");

            Assert.AreEqual("token", repository.Token);
            Assert.AreEqual(1, repository.UserId);

            this.tokenStore.Verify(store => store.Set(authEnvelope.AccessToken.Token), Times.Once, "Login should save token");
            Assert.IsTrue(this.settings.ContainsKey("CurrentUserId"), "Login should save user id in settings");
        }

        [TestMethod]
        public void logout_clears_token()
        {
            getRepository().Logout();

            this.tokenStore.Verify(store => store.Remove(), Times.Once, "Token should be cleared on logout");
        }

        [TestMethod]
        public void autologin_retrieves_from_settings()
        {
            this.settings.AddOrUpdate("CurrentUserId", 1);
            this.tokenStore.SetupGet(x => x.Token).Returns("token");
            var repository = getRepository();

            repository.AutoLogin();

            Assert.AreEqual("token", repository.Token);
            //Assert.AreEqual(1, repository.UserId); 
        }

        public IIdentityStore getRepository()
        {
            return new IdentityStore(this.userService.Object, this.settings, this.tokenStore.Object);
        }
    }
}
