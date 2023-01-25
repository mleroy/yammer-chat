using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly INavigator navigator;
        private readonly IIdentityStore identityStore;

        public ShellViewModel(INavigator navigator, IIdentityStore identityStore)
        {
            this.navigator = navigator;
            this.identityStore = identityStore;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            GoToLandingView();
        }

        public void GoToLandingView()
        {
            if (!this.identityStore.IsLoggedIn)
            {
                this.navigator.Navigate<OAuthLoginViewModel>();
            }
            else
            {
                this.navigator.Navigate<ThreadsViewModel>();
            }

            this.navigator.RemoveBackEntry();
        }
    }
}
