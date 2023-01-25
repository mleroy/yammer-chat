using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Test.Mocks;

namespace Yammer.Chat.Core.Test
{
    [TestClass]
    public class TokenStoreTests : TestBase
    {
        private SettingsMock settings;
        private CryptographerMock cryptographer;

        [TestInitialize]
        public void Init()
        {
            this.settings = new SettingsMock();
            this.cryptographer = new CryptographerMock();
        }

        [TestMethod]
        public void ShouldSetToken()
        {
            var tokenStore = getTokenStore();

            tokenStore.Set("foo");
            Assert.AreEqual("foo", tokenStore.Token);
        }

        [TestMethod]
        public void ShouldRemoveToken()
        {
            var tokenStore = getTokenStore();
            tokenStore.Set("foo");
            tokenStore.Remove();

            Assert.IsFalse(tokenStore.HasToken);
        }

        [TestMethod]
        public void ShouldOverwrite()
        {
            var tokenStore = getTokenStore();
            tokenStore.Set("foo");
            tokenStore.Set("foo2");

            Assert.AreEqual(tokenStore.Token, "foo2");
        }

        private ITokenStore getTokenStore()
        {
            return new TokenStore(this.settings, this.cryptographer);
        }
    }
}
