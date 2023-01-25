using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API
{
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
    }
}
