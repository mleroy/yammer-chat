using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.ViewModels.Design
{
    public  class UserDesignViewModel : User
    {
        public UserDesignViewModel()
            : base()
        {
            this.Presence = UserPresence.Online;
        }
    }
}
