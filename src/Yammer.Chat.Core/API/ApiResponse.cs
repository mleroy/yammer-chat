using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API
{
    public interface IApiResponse
    {
        bool HasContent { get; }
        T ToEntity<T>(IApiDeserializer deserializer = null) where T : class;
    }

    public class ApiResponse : IApiResponse
    {
        private readonly byte[] content;

        public ApiResponse(byte[] content)
        {
            this.content = content;
        }

        public bool HasContent
        {
            get { return content.Length > 0; }
        }

        public T ToEntity<T>(IApiDeserializer deserializer = null)
            where T : class
        {
            if (deserializer == null)
                deserializer = new DefaultSerializer();

            return deserializer.Deserialize<T>(content);
        }
    }
}
