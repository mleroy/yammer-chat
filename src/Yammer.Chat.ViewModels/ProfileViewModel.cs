using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private readonly IUserRepository userRepository;
        private readonly IProgressIndicator progressIndicator;

        /// <summary>
        /// Navigation parameters
        /// </summary>
        public long UserId { get; set; }

        public ProfileViewModel(IUserRepository userRepository, IProgressIndicator progressIndicator)
        {
            this.userRepository = userRepository;
            this.progressIndicator = progressIndicator;
        }

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            using (this.progressIndicator.Show())
            {
                this.User = await this.userRepository.GetUser(this.UserId);
            }
        }

        public User User
        {
            get { return this.user; }
            set { base.SetProperty(ref this.user, value); }
        }
        private User user;
    }
}
