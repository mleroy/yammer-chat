using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.Repositories
{
    public interface ITokenStore
    {
        bool HasToken { get; }
        string Token { get; }

        void Set(string token);
        void Remove();
    }

    public class TokenStore : ITokenStore
    {
        private const string TokenKey = "_token_";

        private readonly ISettings settings;
        private readonly ICryptographer cryptographer;

        public TokenStore(ISettings settings, ICryptographer cryptographer)
        {
            this.settings = settings;
            this.cryptographer = cryptographer;
        }

        public string Token
        {
            get
            {
                byte[] encryptedToken;

                if (!this.settings.TryGetValue(TokenKey, out encryptedToken))
                    return null;

                return this.cryptographer.Decrypt(encryptedToken);
            }
        }

        public void Set(string token)
        {
            var encryptedToken = this.cryptographer.Encrypt(token);
            this.settings.AddOrUpdate(TokenKey, encryptedToken);
        }

        public void Remove()
        {
            this.settings.Remove(TokenKey);
        }

        public bool HasToken
        {
            get { return this.settings.ContainsKey(TokenKey); }
        }
    }
}
