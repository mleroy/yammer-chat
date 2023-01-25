using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API.Dtos
{
    public class UsersEnvelope
    {
        [JsonProperty("users")]
        public UserDto[] Users { get; set; }
    }
}
