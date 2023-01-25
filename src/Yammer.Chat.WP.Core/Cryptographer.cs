using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;

namespace Yammer.Chat.WP.Core
{
    public class Cryptographer : ICryptographer
    {
        byte[] ICryptographer.Encrypt(string value)
        {
            return ProtectedData.Protect(Encoding.UTF8.GetBytes(value), new byte[0]);
        }

        string ICryptographer.Decrypt(byte[] encrypted)
        {
            var unprotectedBytes = ProtectedData.Unprotect(encrypted, new byte[0]);
            return Encoding.UTF8.GetString(unprotectedBytes, 0, unprotectedBytes.Length);
        }
    }
}
