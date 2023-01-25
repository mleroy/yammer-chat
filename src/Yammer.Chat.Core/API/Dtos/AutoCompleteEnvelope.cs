using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API.Dtos
{
    public class AutoCompleteEnvelope
    {
        [JsonProperty("user")]
        public AutoCompleteUserDto[] Users { get; set; }
    }

    public class AutoCompleteUserDto
    {
        [JsonProperty("id")]
        public Int64 Id { get; set; }

        [JsonProperty("full_name")]
        public String FullName { get; set; }

        [JsonProperty("job_title")]
        public string JobTitle { get; set; }

        [JsonProperty("mugshot_url_template")]
        public string MugshotTemplate { get; set; }

        [JsonProperty("presence")]
        public string Presence { get; set; }
    }
}
