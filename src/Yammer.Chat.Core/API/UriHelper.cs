using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API
{
    public static class UriHelper
    {
        public static string BuildUriWithParameters(string uri, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            if (parameters != null && parameters.Any())
            {
                var serialized = string.Join("&", parameters.Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)));
                return uri + "?" + serialized;
            }

            return uri;
        }

        public static Dictionary<string, string> ExtractParameters(Uri uri)
        {
            var serialized = uri.Query.Substring(1); // Get rid of "?"

            var parameters = from pair in serialized.Split('&')
                             let parts = pair.Split('=')
                             where parts.Count() == 2
                             select new KeyValuePair<string, string>(parts[0], parts[1]);

            var dictionary = new Dictionary<string, string>();

            foreach (var kv in parameters)
            {
                dictionary.Add(kv.Key, kv.Value);
            }

            return dictionary;
        }
    }
}
