using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.Models
{
    [DebuggerDisplay("Thread #{Id}")]
    public class Thread : PropertyChangedBase, IEquatable<Thread>, IEqualityComparer<Thread>
    {
        public Thread()
        {
            this.Messages = new ObservableCollection<Message>();
            this.Participants = new ObservableCollection<User>();

            // Allows binding to the Participants property and getting notification when collection changes, instead of property change
            this.Participants.CollectionChanged += (s, e) => base.NotifyOfPropertyChange(() => this.Participants);
        }

        public long Id { get; set; }

        public ObservableCollection<Message> Messages { get; set; }

        public Message LastMessage { get { return this.Messages.Last(); } }

        public ObservableCollection<User> Participants { get; set; }

        public int TotalMessages { get; set; }

        public long? FirstReplyId { get; set; }

        public bool IsUnread
        {
            get { return this.isUnread; }
            set
            {
                this.isUnread = value;
                base.NotifyOfPropertyChange(() => this.IsUnread);
            }
        }
        private bool isUnread;

        public void AddMessage(Message message)
        {
            if (!this.Messages.Contains(message))
            {
                for (var i = 0; i < this.Messages.Count; i++)
                {
                    if (message.Id < this.Messages[i].Id)
                    {
                        this.Messages.Insert(i, message);
                        break;
                    }
                }

                if (!this.Messages.Contains(message))
                {
                    this.Messages.Add(message);
                }

                this.TotalMessages++;

                this.NotifyOfPropertyChange(() => this.LastMessage);
            }
        }

        public void AddParticipant(User participant)
        {
            if (!this.Participants.Contains(participant))
            {
                this.Participants.Add(participant);
            }
        }

        public void Merge(Thread thread)
        {
            this.TotalMessages = thread.TotalMessages;

            // If the newest message we have is older than the oldest message of the thread to merge, we would introduce a gap of unknown size
            // To prevent this, clear current messages
            if (!thread.Messages.Contains(this.LastMessage))
            {
                this.Messages.Clear();
            }

            var newMessages = thread.Messages.Except(this.Messages).OrderBy(m => m.Id).ToList();

            foreach (var message in newMessages)
            {
                this.Messages.Add(message);
            }

            if (newMessages.Any())
            {
                this.NotifyOfPropertyChange(() => this.LastMessage);
            }

            foreach (var participant in thread.Participants.Except(this.Participants))
            {
                this.Participants.Add(participant);
            }

            this.IsUnread = thread.IsUnread;
        }

        public bool IsDraft { get { return this.Id == 0; } }

        public bool Equals(Thread other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Thread);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public bool Equals(Thread x, Thread y)
        {
            if (object.ReferenceEquals(x, y)) return true;

            return x != null && y != null && x.Equals(y);
        }

        public int GetHashCode(Thread obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
