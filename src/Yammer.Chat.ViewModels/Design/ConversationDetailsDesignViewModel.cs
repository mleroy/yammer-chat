using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.ViewModels.Design
{
    public class ConversationDetailsDesignViewModel : ConversationDetailsViewModel
    {
        public ConversationDetailsDesignViewModel()
            : base(null, null, null, null)
        {
            this.Thread = new Thread();

            this.Thread.Participants = new ObservableCollection<User> 
            {
                new User { FullName = "Jean Pierre", MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX", Presence = UserPresence.Offline },
                new User { FullName = "Person with a very very very very very long name.", MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX", Presence = UserPresence.Online },
                new User { FullName = "Gaston Therrien", MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX", Presence = UserPresence.Mobile },
                new User { FullName = "Robert Charlebois", MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX", Presence = UserPresence.Unknown },
            };
        }
    }
}
