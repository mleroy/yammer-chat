using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yammer.Chat.Core.Repositories;

namespace Yammer.Chat.Core.API
{
    public interface IHttpService
    {
        void SetDecorator(Action<HttpClient> decorator);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
        Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption);
    }

    public class HttpService : IHttpService
    {
        private readonly HttpClient httpClient;

        private Action<HttpClient> httpClientDecorator;

        public HttpService(IHttpClientProvider httpClientProvider)
        {
            this.httpClient = httpClientProvider.Create();
        }

        public void SetDecorator(Action<HttpClient> decorator)
        {
            this.httpClientDecorator = decorator;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            this.Decorate(this.httpClient);

            return this.httpClient.SendAsync(request, cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
        {
            this.Decorate(this.httpClient);

            return this.httpClient.GetAsync(requestUri, completionOption);
        }

        private void Decorate(HttpClient httpClient)
        {
            if (this.httpClientDecorator != null)
            {
                this.httpClientDecorator(httpClient);
            }
        }
    }
}
