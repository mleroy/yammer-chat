using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API.Dtos.Converters
{
    public enum AttachmentType { Unknown, Image };

    public class AttachmentConverter : JsonCreationConverter<AttachmentDto>
    {
        protected override AttachmentDto Create(Type objectType, JObject jObject)
        {
            AttachmentType type;

            Enum.TryParse<AttachmentType>(jObject["type"].ToString(), true, out type);

            switch (type)
            {
                case AttachmentType.Image:
                    return new ImageAttachmentDto();
                default:
                    return new AttachmentDto();
            }
        }
    }
}
