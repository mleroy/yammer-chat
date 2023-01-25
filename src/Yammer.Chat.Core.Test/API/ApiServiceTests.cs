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
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Repositories;

namespace Yammer.Chat.Core.Test.API
{
    [TestClass]
    public class ApiServiceTests : TestBase
    {
        private IClientConfiguration clientConfiguration;
        private IHttpService httpService;

        [TestInitialize]
        public void Init()
        {
            this.clientConfiguration = new DefaultClientConfiguration();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task UnauthenticatedRequest()
        {
            var service = getService();

            var response = await service.GetAsync("http://www.google.com");

            Assert.IsTrue(response.HasContent);
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task AuthenticatedRequest()
        {
            var service = getService();
            this.httpService.SetDecorator(httpClient =>
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Nv1ZLLSoL9ZhEjpnHA0ebA");
            });

            var response = await service.GetAsync("/api/v1/users/current.json");
            Assert.AreEqual("Matt", response.ToEntity<UserDto>().FirstName);
        }

        public IApiService getService()
        {
            var httpClientProvider = new HttpClientProvider(this.clientConfiguration);

            this.httpService = new HttpService(httpClientProvider);

            return new ApiService(this.httpService);
        }
    }
}
