using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.Repositories;
using Yammer.Chat.Core.Resources;

namespace Yammer.Chat.Core.Models
{
    public class Message : PropertyChangedBase, IEquatable<Message>, IEqualityComparer<Message>
    {
        public long Id { get; set; }
        public long ThreadId { get; set; }
        public User Sender { get; set; }
        public string Body
        {
            get
            {
                if (this.BodyParts == null)
                {
                    return null;
                }

                return string.Join("", this.BodyParts.Select(part => part.Text));
            }
        }
        public MessagePart[] BodyParts { get; set; }
        public DateTime Timestamp { get; set; }
        public string MugshotTemplate { get; set; }
        public ObservableCollection<User> Likers { get; set; }

        public ICollection<Attachment> Attachments { get; set; }
        public ImageAttachment FirstImageAttachment
        {
            get
            {
                if (!this.HasImageAttachment)
                    return null;

                return (ImageAttachment)this.Attachments.First(x => x is ImageAttachment);
            }
        }

        public bool HasBody { get { return this.BodyParts != null && this.BodyParts.Length > 0; } }
        public bool HasImageAttachment { get { return this.Attachments != null && this.Attachments.Any(x => x is ImageAttachment); } }
        public bool IsImageWithoutBody
        {
            get { return !this.HasBody && this.HasImageAttachment; }
        }

        public bool IsFromCurrentUser { get; set; }

        // Model could be smart and know whether it is liked by the current user, but it would require injecting a repository which seems unpractical
        public bool IsLikedByCurrentUser
        {
            get { return this.isLikedByCurrentUser; }
            set
            {
                this.isLikedByCurrentUser = value;
                NotifyOfPropertyChange(() => this.IsLikedByCurrentUser);
            }
        }
        private bool isLikedByCurrentUser;

        public void LikedBy(User user)
        {
            this.Likers.Add(user);
            NotifyOfPropertyChange(() => this.LikedByText);
        }

        public void UnlikedBy(User user)
        {
            this.Likers.Remove(user);
            NotifyOfPropertyChange(() => this.LikedByText);
        }

        public ClientType ClientType { get; set; }

        public bool IsSentFromWeb { get { return this.ClientType == ClientType.Web; } }
        public bool IsSentFromMobile { get { return this.ClientType == ClientType.Mobile; } }

        /// <summary>
        /// Binding listeners aren't notified when the Likers collection changes (only if it replaced).
        /// This property is something listeners can bind to and that we will notify explicitly this model changes the collection.
        /// Another approach would be to notify of a change to the Likers property each time the collection changes (and have an Enumerable to String UI converter), but it feels wasteful.
        /// </summary>
        public string LikedByText
        {
            get
            {
                switch (this.Likers.Count)
                {
                    case 0:
                        return null;
                    case 1:
                        return this.Likers.First().FullName;
                    default:
                        return string.Format("{0} {1} {2}",
                            string.Join(", ", this.Likers.Take(this.Likers.Count - 1).Select(x => x.FullName)),
                            AppResources.StringConcatenatorText,
                            this.Likers.Last().FullName);
                }
            }
        }

        public bool Equals(Message other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Message);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public bool Equals(Message x, Message y)
        {
            if (object.ReferenceEquals(x, y)) return true;

            return x != null && y != null && x.Equals(y);
        }

        public int GetHashCode(Message obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
