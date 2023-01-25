using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.ViewModels.Design
{
    public class ParticipantSelectionDesignViewModel : ParticipantSelectionViewModel
    {
        public ParticipantSelectionDesignViewModel()
            : base(null, null, null, null)
        {
            this.Users = new ObservableCollection<User> {
                new User { 
                    FullName = "Jean Pierre", 
                    MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX", 
                    Presence = UserPresence.Offline,
                    IsSelected = true },
                new User { FullName = "Person with a very very very very very long name.", MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX", Presence = UserPresence.Online },
                new User { FullName = "Gaston Therrien", MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX", Presence = UserPresence.Mobile },
                new User { FullName = "Robert Charlebois", MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX", Presence = UserPresence.Unknown },
            };

            this.ToggleUserSelection(this.Users.First());
        }
    }
}
