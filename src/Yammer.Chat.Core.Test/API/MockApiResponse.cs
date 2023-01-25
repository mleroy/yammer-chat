using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.API;

namespace Yammer.Chat.Core.Test.API
{
    public class MockApiResponse : IApiResponse
    {
        private readonly object entity;

        public MockApiResponse(object entity)
        {
            this.entity = entity;
        }

        public T ToEntity<T>(IApiDeserializer deserializer = null)
            where T : class
        {
            return (T)entity;
        }

        public bool HasContent
        {
            get { return true; }
        }
    }
}
