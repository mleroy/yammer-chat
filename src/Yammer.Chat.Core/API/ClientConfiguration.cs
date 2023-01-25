using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.API
{
    public interface IClientConfiguration
    {
        Uri BaseUri { get; }
        
        string ClientId { get; }
        string ClientSecret { get; }

        string ProductName { get; }
        string ProductVersion { get; }

        string OAuthCallbackUri { get; }
    }

    public class DefaultClientConfiguration : IClientConfiguration
    {
        public DefaultClientConfiguration()
        {
            BaseUri = new Uri("https://www.yammer.com");

            // Registered to "Yammer Now for Windows Phone", not trusted for ROPC
            ClientId = "";
            ClientSecret = "";

            ProductName = "Yammer.Now.WindowsPhone";
            ProductVersion = "1.0.2";

            OAuthCallbackUri = "yammernow://oauthcallback";
        }

        public Uri BaseUri { get; private set; }

        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }

        public string ProductName { get; private set; }
        public string ProductVersion { get; private set; }

        public string OAuthCallbackUri { get; private set; }
    }
}
