using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API
{
    public interface IApiService
    {
        Task<IApiResponse> GetAsync(string endpoint, IEnumerable<KeyValuePair<string, string>> queryParameters = null);
        Task<IApiResponse> GetAsync(string endpoint, IEnumerable<KeyValuePair<string, string>> queryParameters, CancellationToken cancellationToken);
        Task<IApiResponse> PostAsync(string endpoint, object bodyParameters = null, ApiRequestSettings settings = null);
        Task<IApiResponse> PutAsync(string endpoint, object bodyParameters = null, ApiRequestSettings settings = null);
        Task<IApiResponse> DeleteAsync(string endpoint, IEnumerable<KeyValuePair<string, string>> parameters = null, ApiRequestSettings settings = null);
    }

    public class ApiService : IApiService
    {
        private readonly IHttpService httpService;

        public ApiService(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public Task<IApiResponse> GetAsync(string endpoint, IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            return execute(HttpMethod.Get, endpoint, queryParameters, null, null, CancellationToken.None);
        }

        public Task<IApiResponse> GetAsync(string endpoint, IEnumerable<KeyValuePair<string, string>> queryParameters, CancellationToken cancellationToken)
        {
            return execute(HttpMethod.Get, endpoint, queryParameters, null, null, cancellationToken);
        }

        public Task<IApiResponse> PostAsync(string endpoint, object bodyParameters, ApiRequestSettings settings)
        {
            return execute(HttpMethod.Post, endpoint, null, bodyParameters, settings, CancellationToken.None);
        }

        public Task<IApiResponse> PutAsync(string endpoint, object bodyParameters, ApiRequestSettings settings)
        {
            return execute(HttpMethod.Put, endpoint, null, bodyParameters, settings, CancellationToken.None);
        }

        public Task<IApiResponse> DeleteAsync(string endpoint, IEnumerable<KeyValuePair<string, string>> parameters, ApiRequestSettings settings)
        {
            return execute(HttpMethod.Delete, endpoint, parameters, null, settings, CancellationToken.None);
        }

        private async Task<IApiResponse> execute(
                HttpMethod method,
                string endpoint,
                IEnumerable<KeyValuePair<string, string>> queryParameters,
                object bodyParameters,
                ApiRequestSettings settings,
                CancellationToken cancellationToken)
        {
            if (settings == null)
                settings = new ApiRequestSettings();

            var uri = UriHelper.BuildUriWithParameters(endpoint, queryParameters);

            using (var request = new HttpRequestMessage(method, uri))
            {
                HttpContent httpContent = null;

                if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put)
                {
                    request.Content = settings.Serialize(bodyParameters);
                }

                var timedAnalyticsEvent = Analytics.Default.StartTimedEvent("ApiRequest");
                timedAnalyticsEvent.Properties.Add("method", method.ToString());
                timedAnalyticsEvent.Properties.Add("endpoint", endpoint);

                var response = await httpService.SendAsync(request, cancellationToken);

                timedAnalyticsEvent.End();

                if (httpContent != null)
                    httpContent.Dispose();

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpException { StatusCode = response.StatusCode };
                }

                var responseContent = await response.Content.ReadAsByteArrayAsync();

                return new ApiResponse(responseContent);
            }
        }
    }
}
