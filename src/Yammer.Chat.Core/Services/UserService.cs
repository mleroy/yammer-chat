using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Yammer.Chat.Core.API;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Exceptions;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.Core.Services
{
    public interface IUserService
    {
        Task<AuthEnvelope> AuthenticateAsync(string username, string password);
        Task<UserDto> GetCurrentUser();
        Task<UserDto> GetUser(long id);
        Task UpdateUser(User user);
        Task UpdateUserMugshot(long userId, string mugshotId);
        Task<UsersEnvelope> GetPresences(IEnumerable<long> userIds);

        Task<IEnumerable<AutoCompleteUserDto>> GetAutocompleteUsers(string prefix, CancellationToken cancellationToken);
    }

    public class UserService : IUserService
    {
        private readonly IApiService api;
        private readonly IClientConfiguration configuration;

        public UserService(IApiService api, IClientConfiguration configuration)
        {
            this.api = api;
            this.configuration = configuration;
        }

        public async Task<AuthEnvelope> AuthenticateAsync(string username, string password)
        {
            var parameters = new Dictionary<string, string>()
            { 
                { "username", username },
                { "password", password },
                { "client_id", configuration.ClientId },
                { "client_secret", configuration.ClientSecret }
            };

            try
            {
                var response = await api.PostAsync("/oauth2/access_token.json", parameters);
                return response.ToEntity<AuthEnvelope>();
            }
            catch (HttpException e)
            {
                switch (e.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        throw new InvalidCredentialsException();
                    case HttpStatusCode.Forbidden:
                        throw new SsoNetworkException();
                    default:
                        throw new Exception();
                }
            }
        }

        public async Task<UserDto> GetUser(long id)
        {
            var endpoint = string.Format("/api/v1/users/{0}.json", id);

            var response = await api.GetAsync(endpoint);
            return response.ToEntity<UserDto>();
        }

        public async Task<UserDto> GetCurrentUser()
        {
            var response = await api.GetAsync("/api/v1/users/current.json");
            return response.ToEntity<UserDto>();
        }

        public async Task UpdateUser(User user)
        {
            var endpoint = string.Format("/api/v1/users/{0}.json", user.Id);

            var parameters = new Dictionary<string, string>()
            { 
                { "first_name", user.FirstName },
                { "last_name", user.LastName },
                { "full_name", user.FullName },
                { "job_title", user.JobTitle },
                { "work_telephone", user.WorkPhone },
                { "mobile_telephone", user.MobilePhone },
                { "summary", user.Summary }
            };

            await this.api.PutAsync(endpoint, parameters);
        }

        public Task UpdateUserMugshot(long userId, string mugshotId)
        {
            var endpoint = string.Format("/api/v1/users/{0}.json", userId);

            var parameters = new Dictionary<string, string>()
            { 
                { "mugshot_id", mugshotId }
            };

            return this.api.PutAsync(endpoint, parameters);
        }

        public async Task<UsersEnvelope> GetPresences(IEnumerable<long> userIds)
        {
            var endpoint = string.Format("/api/v1/presences/{0}.json", string.Join(",", userIds));

            var response = await this.api.GetAsync(endpoint);
            return response.ToEntity<UsersEnvelope>();
        }

        public async Task<IEnumerable<AutoCompleteUserDto>> GetAutocompleteUsers(string prefix, CancellationToken cancellationToken)
        {
            var count = string.IsNullOrEmpty(prefix) ? 20 : 5;

            var parameters = new Dictionary<string, string>()
            { 
                { "models", string.Format("user:{0}", count) },
                { "presence", "true" }
            };

            if (!string.IsNullOrEmpty(prefix))
            {
                parameters.Add("prefix", prefix);
            }

            var response = await this.api.GetAsync("/api/v1/autocomplete/ranked", parameters, cancellationToken);
            return response.ToEntity<AutoCompleteEnvelope>().Users;
        }
    }
}
