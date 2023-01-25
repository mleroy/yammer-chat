using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API
{
    public class ApiRequestSettings
    {
        private readonly static DefaultSerializer DefaultSerializer = new DefaultSerializer();

        private IApiSerializer serializer;
        private IApiDeserializer deserializer;

        public ApiRequestSettings(IApiSerializer serializer = null, IApiDeserializer deserializer = null)
        {
            this.serializer = serializer ?? DefaultSerializer;
            this.deserializer = deserializer ?? DefaultSerializer;
        }
        
        public ApiRequestSettings WithSerializer(IApiSerializer serializer)
        {
            this.serializer = serializer;
            return this;
        }

        public ApiRequestSettings WithDeserializer(IApiDeserializer deserializer)
        {
            this.deserializer = deserializer;
            return this;
        }

        public HttpContent Serialize(object obj) { return serializer.Serialize(obj); }
        public T Deserialize<T>(byte[] bytes) { return deserializer.Deserialize<T>(bytes); }
    }
}
