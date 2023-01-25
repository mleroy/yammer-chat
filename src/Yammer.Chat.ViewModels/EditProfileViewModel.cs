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
    public class EditProfileViewModel : ViewModelBase
    {
        private readonly IUserRepository userRepository;
        private readonly INavigator navigator;
        private readonly IProgressIndicator progressIndicator;
        private readonly IFileRepository fileRepository;
        private readonly IPhotoChooser photoChooser;

        public EditProfileViewModel(IUserRepository userRepository, INavigator navigator, IProgressIndicator progressIndicator, IFileRepository fileRepository, IPhotoChooser photoChooser)
        {
            this.userRepository = userRepository;
            this.navigator = navigator;
            this.progressIndicator = progressIndicator;
            this.fileRepository = fileRepository;
            this.photoChooser = photoChooser;
        }

        protected async override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            this.CurrentUser = await this.userRepository.GetCurrentUser();
        }

        public async Task Save()
        {
            try
            {
                using (this.progressIndicator.Show(AppResources.ProfileSavingText))
                {
                    await this.userRepository.UpdateCurrentUser(this.FirstName, this.LastName, this.JobTitle, this.Summary, this.WorkPhone, this.MobilePhone);
                }

                this.navigator.GoBack();

                Analytics.Default.LogEvent("Profile/Saving/Success");
            }
            catch (Exception e)
            {
                Analytics.Default.LogEvent("Profile/Saving/Error", new Dictionary<string, object> { { "message", e.Message } });
            }
        }

        public void Cancel()
        {
            this.navigator.GoBack();
        }

        public async Task EditMugshot()
        {
            var photo = await this.getPhoto();

            if (photo != null)
            {
                using (this.progressIndicator.Show(AppResources.LoadingMugshotUploadText))
                {
                    this.IsBusy = true;

                    try
                    {
                        var mugshot = await this.fileRepository.UploadMugshot(photo);

                        await this.userRepository.UpdateCurrentUserMugshot(mugshot);

                        this.MugshotTemplate = this.CurrentUser.MugshotTemplate;

                        Analytics.Default.LogEvent("Profile/SavingMugshot/Success");
                    }
                    catch (Exception e)
                    {
                        Analytics.Default.LogEvent("Profile/SavingMugshot/Error", new Dictionary<string, object> { { "message", e.Message } });
                    }

                    this.IsBusy = false;
                }
            }
        }

        private async Task<PhotoChooserResult> getPhoto()
        {
            try
            {
                return await this.photoChooser.GetPhoto();
            }
            catch (TaskCanceledException)
            {
                return null;
            }
        }

        public string FirstName
        {
            get { return this.firstName; }
            set { base.SetProperty(ref this.firstName, value); }
        }
        private string firstName;

        public string LastName
        {
            get { return this.lastName; }
            set { base.SetProperty(ref this.lastName, value); }
        }
        private string lastName;

        public string JobTitle
        {
            get { return this.jobTitle; }
            set { base.SetProperty(ref this.jobTitle, value); }
        }
        private string jobTitle;

        public string Summary
        {
            get { return this.summary; }
            set { base.SetProperty(ref this.summary, value); }
        }
        private string summary;

        public string WorkPhone
        {
            get { return this.workPhone; }
            set { base.SetProperty(ref this.workPhone, value); }
        }
        private string workPhone;

        public string MobilePhone
        {
            get { return this.mobilePhone; }
            set { base.SetProperty(ref this.mobilePhone, value); }
        }
        private string mobilePhone;

        public string MugshotTemplate
        {
            get { return this.mugshotTemplate; }
            set { base.SetProperty(ref this.mugshotTemplate, value); }
        }
        private string mugshotTemplate;

        public User CurrentUser
        {
            get { return this.currentUser; }
            set
            {
                base.SetProperty(ref this.currentUser, value);

                this.FirstName = currentUser.FirstName;
                this.LastName = currentUser.LastName;
                this.JobTitle = currentUser.JobTitle;
                this.WorkPhone = currentUser.WorkPhone;
                this.MobilePhone = currentUser.MobilePhone;
                this.Summary = currentUser.Summary;
                this.MugshotTemplate = currentUser.MugshotTemplate;
            }
        }
        private User currentUser;

        public bool CanEditMugshot
        {
            get { return !this.IsBusy; }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                base.SetProperty(ref this.isBusy, value);

                NotifyOfPropertyChange(() => this.CanEditMugshot);
            }
        }
        private bool isBusy;
    }
}
