using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.Models
{
    public class RealtimeInfo
    {
        public string Uri { get; set; }
        public string PrimaryChannelId { get; set; }
        public string SecondaryChannelId { get; set; }
    }
}
