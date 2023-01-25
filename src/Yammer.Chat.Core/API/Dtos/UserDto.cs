using Newtonsoft.Json;
using System;

namespace Yammer.Chat.Core.API.Dtos
{
    public class UserDto
    {
        [JsonProperty("id")]
        public Int64 Id { get; set; }
        
        [JsonProperty("name")]
        public String Name { get; set; }

        [JsonProperty("first_name")]
        public String FirstName { get; set; }

        [JsonProperty("last_name")]
        public String LastName { get; set; }

        [JsonProperty("full_name")]
        public String FullName { get; set; }

        [JsonProperty("contact")]
        public UserContact ContactInfo { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("job_title")]
        public string JobTitle { get; set; }

        [JsonProperty("mugshot_url_template")]
        public string MugshotTemplate { get; set; }

        [JsonProperty("presence")]
        public UserPresenceDto Presence { get; set; }
    }

    public class UserPresenceDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
