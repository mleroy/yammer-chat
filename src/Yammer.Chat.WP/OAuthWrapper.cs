using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.API;
using Yammer.OAuthSDK.Model;
using Yammer.OAuthSDK.Utils;

namespace Yammer.Chat.WP
{
    public class OAuthWrapper : IOAuthWrapper
    {
        private readonly IClientConfiguration clientConfiguration;

        public OAuthWrapper(IClientConfiguration clientConfiguration)
        {
            this.clientConfiguration = clientConfiguration;
        }

        public void LaunchLogin()
        {
            OAuthUtils.LaunchSignIn(this.clientConfiguration.ClientId, this.clientConfiguration.OAuthCallbackUri);
        }

        public void HandleApprove(string code, string state, Action<string, long> onSuccess, Action onCSRF = null, Action<string> onErrorResponse = null, Action<Exception> onException = null)
        {
            Action<AuthenticationResponse> onError = (authenticationResponse) =>
            {
                if (onErrorResponse != null)
                {
                    onErrorResponse(authenticationResponse.OAuthError.Message);
                }
            };

            OAuthUtils.HandleApprove(this.clientConfiguration.ClientId, this.clientConfiguration.ClientSecret, code, state, onSuccess, onCSRF, onError, onException);
        }

        public void DeleteStoredToken()
        {
            OAuthUtils.DeleteStoredToken();
        }
    }
}
