using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API;
using Yammer.Chat.Core.Parsers;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Services;

namespace Yammer.Chat.Core.Test.Services
{
    [TestClass]
    public class UserServiceTests : TestBase
    {
        private IApiService apiService;
        private IClientConfiguration clientConfiguration;

        private IHttpService httpService;

        [TestInitialize]
        public void Init()
        {
            this.clientConfiguration = new DefaultClientConfiguration();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task ShouldAuthenticate()
        {
            var userService = getService();
            var envelope = await userService.AuthenticateAsync("matt@ozzworks.com", "swanky");
            Assert.AreEqual(envelope.User.FirstName, "Matt");
        }

        [TestCategory("Integration")]
        [TestMethod]
        public async Task ShouldGetCurrentUser()
        {
            var service = getService();
            this.httpService.SetDecorator(httpClient =>
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Nv1ZLLSoL9ZhEjpnHA0ebA");
            });

            var user = await service.GetCurrentUser();
            Assert.AreEqual(user.FirstName, "Matt");
        }

        public IUserService getService()
        {
            var httpClientProvider = new HttpClientProvider(this.clientConfiguration);
            this.httpService = new HttpService(httpClientProvider);
            this.apiService = new ApiService(this.httpService);

            return new UserService(this.apiService, this.clientConfiguration);
        }
    }
}
