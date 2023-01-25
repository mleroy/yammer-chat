using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Services;

namespace Yammer.Chat.Core.Repositories
{
    public interface IIdentityStore
    {
        long UserId { get; set; }
        string Token { get; set; }

        Task LoginAsync(string user, string password);
        Task LoginAsync(string token);
        void Login(string token, long userId);

        void AutoLogin();
        bool IsLoggedIn { get; }
        void Logout();

    }

    public class IdentityStore : IIdentityStore
    {
        private readonly ISettings settings;
        private readonly IUserService userService;
        private readonly ITokenStore tokenStore;

        private const string UserIdSettingsKey = "CurrentUserId";

        public IdentityStore(IUserService userService, ISettings settings, ITokenStore tokenStore)
        {
            this.userService = userService;
            this.tokenStore = tokenStore;
            this.settings = settings;
        }

        public async Task LoginAsync(string user, string password)
        {
            var authEnvelope = await this.userService.AuthenticateAsync(user, password);

            this.UserId = authEnvelope.User.Id;
            this.Token = authEnvelope.AccessToken.Token;
        }

        public async Task LoginAsync(string token)
        {
            this.Token = token;

            var userDto = await this.userService.GetCurrentUser();
            this.UserId = userDto.Id;
        }

        public void Login(string token, long userId)
        {
            this.Token = token;
            this.UserId = userId;
        }

        public void Logout()
        {
            this.UserId = 0;
            this.Token = null;

            Analytics.Default.AppUserId = null;
        }

        // I don't think it's valuable to keep UserId. Identity is purely defined by token, and user id can be retrieved via several api calls already.
        // If it's useful, we might as well store the entire user object.
        public long UserId
        {
            get { return this.userId; }
            set
            {
                this.userId = value;
                this.settings.AddOrUpdate(UserIdSettingsKey, this.userId);

                Analytics.Default.AppUserId = value.ToString();
            }
        }
        private long userId;

        public string Token
        {
            get { return this.token; }
            set
            {
                this.token = value;

                if (string.IsNullOrEmpty(value))
                {
                    this.tokenStore.Remove();
                }
                else
                {
                    this.tokenStore.Set(value);
                }
            }
        }
        private string token;

        public bool IsLoggedIn
        {
            get
            {
                return !string.IsNullOrEmpty(this.Token);
            }
        }

        public void AutoLogin()
        {
            this.token = this.tokenStore.Token;

            this.settings.TryGetValue(UserIdSettingsKey, out this.userId);

            Analytics.Default.AppUserId = this.userId.ToString();
        }
    }
}
