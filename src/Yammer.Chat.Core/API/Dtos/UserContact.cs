using Newtonsoft.Json;
using System.Linq;

namespace Yammer.Chat.Core.API.Dtos
{
    public class UserContact
    {
        [JsonProperty("phone_numbers")]
        public UserPhoneNumber[] PhoneNumbers { get; set; }
    }
}
