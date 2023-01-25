using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.Models
{
    public enum DetailsLevel { Minimal, Full };

    public enum UserPresence { Unknown, Idle, Online, Offline, Mobile };

    public class User : PropertyChangedBase, IEquatable<User>
    {
        public long Id { get; set; }

        public string FirstName
        {
            get { return this.firstName; }
            set
            {
                this.firstName = value;
                UpdateFullName();
                NotifyOfPropertyChange(() => this.FirstName);
            }
        }
        private string firstName;

        public string LastName
        {
            get { return this.lastName; }
            set
            {
                this.lastName = value;
                UpdateFullName();
                NotifyOfPropertyChange(() => this.LastName);
            }
        }
        private string lastName;

        public string FullName
        {
            get { return this.fullName; }
            set
            {
                this.fullName = value;
                NotifyOfPropertyChange(() => this.FullName);
            }
        }
        private string fullName;

        public string MugshotTemplate
        {
            get { return this.mugshotTemplate; }
            set
            {
                this.mugshotTemplate = value;
                NotifyOfPropertyChange(() => this.MugshotTemplate);
            }
        }
        private string mugshotTemplate;

        public string Summary
        {
            get { return this.summary; }
            set
            {
                this.summary = value;
                NotifyOfPropertyChange(() => this.Summary);
            }
        }
        private string summary;

        public string JobTitle
        {
            get { return this.jobTitle; }
            set
            {
                this.jobTitle = value;
                NotifyOfPropertyChange(() => this.JobTitle);
            }
        }
        private string jobTitle;

        public string WorkPhone
        {
            get { return this.workPhone; }
            set
            {
                this.workPhone = value;
                NotifyOfPropertyChange(() => this.WorkPhone);
            }
        }
        private string workPhone;

        public string MobilePhone
        {
            get { return this.mobilePhone; }
            set
            {
                this.mobilePhone = value;
                NotifyOfPropertyChange(() => this.MobilePhone);
            }
        }
        private string mobilePhone;

        public DetailsLevel AvailableDetails { get; set; }

        public void UpdateFullName()
        {
            this.FullName = string.Format("{0} {1}", this.FirstName, this.LastName);
        }

        public void UpdateMugshotTemplate(string newMugshotId)
        {
            var oldIdFinder = @"/([^/]*)$"; // Everything after the last forward slash

            this.MugshotTemplate = Regex.Replace(this.MugshotTemplate, oldIdFinder, string.Format("/{0}", newMugshotId));
        }

        public UserPresence Presence
        {
            get { return this.presence; }
            set
            {
                this.presence = value;
                this.NotifyOfPropertyChange(() => this.Presence);
                this.NotifyOfPropertyChange(() => this.IsOnMobile);
            }
        }
        private UserPresence presence;

        public bool IsOnMobile { get { return this.Presence == UserPresence.Mobile; } }

        // I don't think this really belongs here. Only used by ParticipantViewModel.
        public bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                this.isSelected = value;
                this.NotifyOfPropertyChange(() => this.IsSelected);
            }
        }
        private bool isSelected;

        internal void Merge(User user)
        {
            if (!string.IsNullOrEmpty(user.FirstName))
                this.FirstName = user.FirstName;

            if (!string.IsNullOrEmpty(user.LastName))
                this.LastName = user.LastName;

            if (!string.IsNullOrEmpty(user.MugshotTemplate))
                this.FullName = user.FullName;

            if (!string.IsNullOrEmpty(user.MugshotTemplate))
                this.MugshotTemplate = user.MugshotTemplate;

            if (!string.IsNullOrEmpty(user.Summary))
                this.Summary = user.Summary;

            if (!string.IsNullOrEmpty(user.JobTitle))
                this.JobTitle = user.JobTitle;

            if (!string.IsNullOrEmpty(user.WorkPhone))
                this.WorkPhone = user.WorkPhone;

            if (!string.IsNullOrEmpty(user.MobilePhone))
                this.MobilePhone = user.MobilePhone;

            if (user.AvailableDetails > this.AvailableDetails)
                this.AvailableDetails = user.AvailableDetails;

            if (user.Presence != UserPresence.Unknown)
                this.Presence = user.Presence;
        }

        public bool Equals(User other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as User);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
