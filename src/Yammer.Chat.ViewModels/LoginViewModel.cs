using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Exceptions;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly INavigator navigator;
        private readonly IProgressIndicator progressIndicator;
        private readonly IIdentityStore identityStore;

        public LoginViewModel(INavigator navigator, IProgressIndicator progressIndicator, IIdentityStore identityStore)
        {
            this.navigator = navigator;
            this.progressIndicator = progressIndicator;

            this.identityStore = identityStore;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            // When navigating to login, assume it's from a logout
            this.navigator.ClearHistory();
        }

        public async Task LoginAsync()
        {
            using (progressIndicator.Show(AppResources.LoadingLogInText))
            {
                if (!ValidateEmail())
                    return;

                try
                {
                    await this.identityStore.LoginAsync(this.Email, this.Password);
                }
                catch (InvalidCredentialsException)
                {
                    this.InvalidText = AppResources.InvalidCredentialsText;
                }
                catch (SsoNetworkException)
                {
                    this.navigator.Navigate<SsoLoginViewModel, string>(vm => vm.Email, this.Email);
                    this.navigator.RemoveBackEntry();
                }
                catch (Exception)
                {
                    this.InvalidText = AppResources.DefaultErrorText;
                }

                if (!IsFormInvalid)
                {
                    this.navigator.Navigate<ThreadsViewModel>();
                    this.navigator.RemoveBackEntry();

                    Analytics.Default.LogEvent("Login");
                }
            }
        }

        public bool ValidateEmail()
        {
            this.InvalidText = string.IsNullOrWhiteSpace(this.Email) ? AppResources.InvalidEmailText : string.Empty;
            return string.IsNullOrEmpty(this.InvalidText);
        }

        public string Email
        {
            get { return this.email; }
            set
            {
                base.SetProperty(ref this.email, value);
            }
        }
        private string email;

        public string Password
        {
            get { return this.password; }
            set
            {
                base.SetProperty(ref this.password, value);
            }
        }
        private string password;

        public string InvalidText
        {
            get { return invalidText; }
            set
            {
                base.SetProperty(ref this.invalidText, value);
                NotifyOfPropertyChange("IsFormInvalid");
            }
        }
        private string invalidText;

        public bool IsFormInvalid
        {
            get { return !string.IsNullOrEmpty(this.InvalidText); }
        }
    }
}
