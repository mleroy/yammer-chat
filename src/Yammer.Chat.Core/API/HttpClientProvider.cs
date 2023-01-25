using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Yammer.Chat.Core.API;
using Yammer.Chat.Core.Repositories;

namespace Yammer.Chat.Core.API
{
    public interface IHttpClientProvider
    {
        HttpClient Create();
    }

    public class HttpClientProvider : IHttpClientProvider
    {
        private readonly IClientConfiguration clientConfiguration;

        private HttpClient httpClient;

        public HttpClientProvider(IClientConfiguration clientConfiguration)
        {
            this.clientConfiguration = clientConfiguration;
        }

        public HttpClient Create()
        {
            if (this.httpClient == null)
            {
                var httpClientHandler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                    UseCookies = false
                };

                this.httpClient = new HttpClient(httpClientHandler)
                {
                    BaseAddress = this.clientConfiguration.BaseUri
                };

                this.httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue(this.clientConfiguration.ProductName, this.clientConfiguration.ProductVersion)));
            }

            return this.httpClient;
        }
    }
}
