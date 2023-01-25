using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.Models
{
    public class Attachment
    {
        public long Id { get; set; }
    }

    public class ImageAttachment : Attachment
    {
        public Uri Preview { get; set; }

        public Uri LargePreview { get; set; }
    }
}
