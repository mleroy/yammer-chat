using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API.Dtos.Converters
{
    public enum ReferenceType { Unknown, User, Guide, Bot, Group, Thread, Conversation, Message, Page, Tag, SharedMessage };

    public class ReferenceConverter : JsonCreationConverter<ReferenceDto>
    {
        protected override ReferenceDto Create(Type objectType, JObject jObject)
        {
            ReferenceType type;

            Enum.TryParse<ReferenceType>(jObject["type"].ToString(), true, out type);

            switch (type)
            {
                case ReferenceType.Conversation:
                    return new ConversationReferenceDto();
                case ReferenceType.Thread:
                    return new ThreadReferenceDto();
                case ReferenceType.User:
                case ReferenceType.Guide:
                case ReferenceType.Bot:
                    return new UserReferenceDto();
                case ReferenceType.Group:
                    return new GroupReferenceDto();
                case ReferenceType.Page:
                case ReferenceType.Tag:
                case ReferenceType.Message:
                case ReferenceType.SharedMessage:
                default:
                    return new ReferenceDto();
            }
        }
    }
}
