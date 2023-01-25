using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.ViewModels.Design
{
    public class ThreadDesignViewModel : ThreadViewModel
    {
        public ThreadDesignViewModel()
            : base(null, null, null, null, null, null, null)
        {
            var messages = new List<Message>();

            for (int i = 0; i < 5; i++)
            {
                var message = new Message
                {
                    Sender = new User { FullName = "Robinson Crusoe" },
                    BodyParts = new[] { new MessagePart { Text = "Chat message #" + i + ". A b c d e f g h i j k l m n o p q r s t u v w x y z." } },
                    Timestamp = DateTime.Now.Subtract(TimeSpan.FromMinutes(23 * i)),
                    MugshotTemplate = i % 2 == 0 ? "" : "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX",
                    IsFromCurrentUser = i % 3 == 0
                };

                if (i % 2 == 0)
                {
                    var likers = new List<User>();
                    likers.Add(new User { FullName = "Jean-Pierre Coutu" });
                    likers.Add(new User { FullName = "Tom Peladeau" });

                    message.Likers = new ObservableCollection<User>(likers);
                }

                messages.Insert(0, message);
            }

            var participants = new List<User>();

            participants.Add(new User
            {
                FirstName = "Barack",
                FullName = "Barack Obama",
                MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/lcvzHx1vfdsTw8vcp5sqWq2Tr6xRBn-f"
            });

            participants.Add(new User
            {
                FirstName = "George",
                FullName = "George Bush",
                MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX"
            });

            this.Thread = new Thread
            {
                Id = 1,
                Participants = new ObservableCollection<User>(participants),
                Messages = new ObservableCollection<Message>(messages),
            };
        }
    }
}
