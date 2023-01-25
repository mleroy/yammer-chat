using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API
{
    public interface IApiSerializer
    {
        HttpContent Serialize(object obj);
    }

    public interface IApiDeserializer
    {
        T Deserialize<T>(byte[] content);
    }

    public class DefaultSerializer : IApiSerializer, IApiDeserializer
    {
        public T Deserialize<T>(byte[] content)
        {
            var asString = UTF8Encoding.UTF8.GetString(content, 0, content.Length);
            return JsonConvert.DeserializeObject<T>(asString);
        }

        public HttpContent Serialize(object obj)
        {
            var parameters = obj as IEnumerable<KeyValuePair<string, string>>;

            if (parameters == null)
            {
                return null;
            }

            return new FormUrlEncodedContent(parameters);
        }
    }
}
