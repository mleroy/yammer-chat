using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.ViewModels.Design
{
    public class EditProfileDesignViewModel : EditProfileViewModel
    {
        public EditProfileDesignViewModel()
            : base(null, null, null, null, null)
        {
            this.CurrentUser = new User
            {
                FirstName = "Bob",
                LastName = "Gratton",
                FullName = "Bob Gratton",
                JobTitle = "Pelleteur de nuages",
                MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX",
                WorkPhone = "+1 (514) 626-9536",
                MobilePhone = "514-661-0462",
                Summary = "I'm a cool guy. I have so much to say about myself! Call me to learn more."
            };
        }
    }
}
