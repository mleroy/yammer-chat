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
    public class MessageControlViewModel : Message
    {
        public Message Message { get; set; }

        public MessageControlViewModel()
            : base()
        {
            this.Message = new Message();

            this.Message.Sender = new User { FullName = "Robinson Crusoe" };
            this.Message.BodyParts = new[] { new MessagePart { Text = "This is a message!" } };
            this.Message.Timestamp = DateTime.Now.Subtract(TimeSpan.FromMinutes(23));
            this.Message.MugshotTemplate = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/gswbsB8cHMvk81cbj0Pv80FD-z8gjXxX";
            this.Message.IsFromCurrentUser = false;

            this.Message.Attachments = new Collection<Attachment>
            {
                new ImageAttachment 
                {
                    Preview = new Uri("http://upload.wikimedia.org/wikipedia/commons/f/f1/Lake_Louise_17092005.jpg")
                }
            };

            var likers = new List<User>();
            likers.Add(new User { FullName = "Jean-Pierre Coutu" });
            likers.Add(new User { FullName = "Tom Peladeau" });

            this.Message.Likers = new ObservableCollection<User>(likers);
        }
    }
}
