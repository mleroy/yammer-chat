using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.Test.Mocks
{
    public class CryptographerMock : ICryptographer
    {
        private string backingStore;

        byte[] ICryptographer.Encrypt(string value)
        {
            this.backingStore = value;
            return new byte[0];
        }

        string ICryptographer.Decrypt(byte[] encrypted)
        {
            return backingStore;
        }
    }
}
