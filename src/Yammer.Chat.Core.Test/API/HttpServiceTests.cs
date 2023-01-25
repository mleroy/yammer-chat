using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API;

namespace Yammer.Chat.Core.Test.API
{
    [TestClass]
    public class HttpServiceTests : TestBase
    {
        private Mock<IHttpClientProvider> httpClientProvider;

        [TestInitialize]
        public void init()
        {
            this.httpClientProvider = new Mock<IHttpClientProvider>();
        }

        [TestMethod]
        public void service_gets_client_from_provider()
        {
            var service = getService();

            this.httpClientProvider.Verify(x => x.Create(), Times.Once, "Should get http client from provider");
        }

        // For the following tests, would need to abstract out HttpClient from provider into IHttpClient.

        //[TestMethod]
        //public void service_executes_decorator()
        //{
        //    var service = getService();
        //}

        //[TestMethod]
        //public void service_makes_authenticated_request_when_logged_in()
        //{

        //}

        //[TestMethod]
        //public void service_makes_unauthenticated_request_when_logged_out()
        //{

        //}

        //[TestMethod]
        //public void service_forgets_authentication_after_logout()
        //{

        //}

        private IHttpService getService()
        {
            return new HttpService(this.httpClientProvider.Object);
        }
    }
}
