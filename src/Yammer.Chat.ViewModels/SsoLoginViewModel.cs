using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.API;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.ViewModels
{
    public class SsoLoginViewModel : ViewModelBase
    {
        private readonly IIdentityStore identityStore;
        private readonly INavigator navigator;
        private readonly IProgressIndicator progressIndicator;
        private readonly IClientConfiguration clientConfiguration;

        /// <summary>
        /// Navigation parameters
        /// </summary>
        public string Email { get; set; }

        public SsoLoginViewModel(IIdentityStore identityStore, INavigator navigator, IProgressIndicator progressIndicator, IClientConfiguration clientConfiguration)
        {
            this.identityStore = identityStore;
            this.navigator = navigator;
            this.progressIndicator = progressIndicator;
            this.clientConfiguration = clientConfiguration;
        }

        public async Task Navigated(Uri uri)
        {
            this.progressIndicator.Hide();

            if (!uri.AbsolutePath.Contains("/sso_session/complete"))
            {
                return;
            }

            var parameters = UriHelper.ExtractParameters(uri);

            if (parameters.ContainsKey("access_token"))
            {
                var token = parameters["access_token"];

                using (this.progressIndicator.Show(AppResources.LoadingLogInText))
                {
                    await this.identityStore.LoginAsync(token);

                    this.navigator.Navigate<ThreadsViewModel>();
                    this.navigator.RemoveBackEntry();

                    Analytics.Default.LogEvent("SsoLogin");
                }
            }
            else
            {
                // TODO: assume error/unsupported case:
                // - log parameters dictionary
                // - instruct user something went wrong

                this.navigator.Navigate<LoginViewModel>();
                this.navigator.RemoveBackEntry();
            }
        }

        public void ShowProgress()
        {
            this.progressIndicator.Show();
        }

        public Uri InitialUri
        {
            get
            {
                var endpoint = string.Format("{0}sso_session/access_token", this.clientConfiguration.BaseUri);

                var parameters = new Dictionary<string, string>
                {
                    { "client_id", this.clientConfiguration.ClientId },
                    { "client_secret", this.clientConfiguration.ClientSecret },
                    { "login", this.Email }
                };

                var address = UriHelper.BuildUriWithParameters(endpoint, parameters);

                return new Uri(address);
            }
        }
    }
}
