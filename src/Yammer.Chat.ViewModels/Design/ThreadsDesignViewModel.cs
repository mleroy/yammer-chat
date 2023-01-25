using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Yammer.Chat.Core.API.Dtos;
using Yammer.Chat.Core.Models;
using Yammer.Chat.Core.Repositories;

namespace Yammer.Chat.ViewModels.Design
{
    public class ThreadsDesignViewModel : ThreadsViewModel
    {
        public new ObservableCollection<Thread> Threads { get; set; }

        public ThreadsDesignViewModel()
            : base(null, null, null, null, null, null)
        {
            this.Threads = new ObservableCollection<Thread>();

            for (int i = 0; i < 20; i++)
            {
                var thread = new Thread { Id = i };

                var message = new Message
                {
                    BodyParts = new[] { new MessagePart { Text = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut" } },
                    Timestamp = DateTime.Now.Subtract(TimeSpan.FromMinutes(23 * i)),
                    MugshotTemplate = i % 2 == 0 ? "" : "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX"
                };

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

                if (i % 3 == 0)
                {
                    participants.Add(new User
                    {
                        FirstName = "Barack",
                        FullName = "Barack Obama",
                        MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/Bdlj9CvRF6R-C38Bb6QK0k5w2Ff6Kzwv"
                    });

                }

                thread.Messages = new ObservableCollection<Message>(new[] { message });
                thread.Participants = new ObservableCollection<User>(participants);
                thread.IsUnread = i % 4 == 0;

                this.Threads.Add(thread);
            }
        }
    }
}
