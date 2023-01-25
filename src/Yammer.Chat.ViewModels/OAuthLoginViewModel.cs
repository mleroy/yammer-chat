using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.ViewModels
{
    public class OAuthLoginViewModel : ViewModelBase
    {
        private readonly IOAuthWrapper oAuthWrapper;
        private readonly INavigator navigator;
        private readonly IIdentityStore identityStore;
        private readonly IProgressIndicator progressIndicator;

        public OAuthLoginViewModel(IOAuthWrapper oAuthWrapper, INavigator navigator, IIdentityStore identityStore, IProgressIndicator progressIndicator)
        {
            this.oAuthWrapper = oAuthWrapper;
            this.navigator = navigator;
            this.identityStore = identityStore;
            this.progressIndicator = progressIndicator;
        }

        public void OAuthLogin()
        {
            this.ErrorDisplayText = "";

            this.oAuthWrapper.LaunchLogin();
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            // When navigating to login, assume it's from a logout
            this.navigator.ClearHistory();

            // "Approve"
            if (!string.IsNullOrEmpty(this.Code) && !string.IsNullOrEmpty(this.State))
            {
                Action<string, long> onSuccess = (token, userId) =>
                {
                    this.identityStore.Login(token, userId);

                    this.progressIndicator.Hide();

                    this.navigator.Navigate<ThreadsViewModel>();
                    this.navigator.RemoveBackEntry();

                    Analytics.Default.LogEvent("Login");
                };

                Action onCSRF = () =>
                {
                    this.progressIndicator.Hide();
                    this.ErrorDisplayText = AppResources.OAuthGenericErrorText;
                };
                Action<string> onError = (message) =>
                {
                    this.progressIndicator.Hide();
                    this.ErrorDisplayText = AppResources.OAuthGenericErrorText;
                };
                Action<Exception> onException = (e) =>
                {
                    this.progressIndicator.Hide();
                    this.ErrorDisplayText = AppResources.OAuthGenericErrorText;
                };

                this.progressIndicator.Show();

                this.oAuthWrapper.HandleApprove(this.Code, this.State, onSuccess, onCSRF, onError, onException);
            }
            // "Deny"
            else if (!string.IsNullOrEmpty(this.Error))
            {
                this.ErrorDisplayText = AppResources.OAuthDeniedErrorText;
                
                this.oAuthWrapper.DeleteStoredToken();
            }
        }

        public string Code { get; set; }
        public string State { get; set; }
        public string Error { get; set; }
        public string ErrorDescription { get; set; }

        public string ErrorDisplayText
        {
            get
            {
                return this.errorDisplayText;
            }
            set
            {
                this.errorDisplayText = value;
                this.NotifyOfPropertyChange(() => this.ErrorDisplayText);
                this.NotifyOfPropertyChange(() => this.ShowError);
            }
        }
        private string errorDisplayText;
        public bool ShowError { get { return !string.IsNullOrEmpty(this.ErrorDisplayText); } }
    }
}
