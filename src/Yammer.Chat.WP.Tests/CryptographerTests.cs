using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.WP.Core;

namespace Yammer.Chat.WP.Tests
{
    [TestClass]
    public class CryptographerTests
    {
        private const string SampleString = "Chartreuse is good for you.";

        [TestMethod]
        public void Encryption()
        {
            var cryptographer = getCryptographer();
            var encrypted = cryptographer.Encrypt(SampleString);

            var plainTextDecryption = Encoding.UTF8.GetString(encrypted, 0, encrypted.Length);

            Assert.AreNotEqual(SampleString, plainTextDecryption, "Encrypted value should be encrypted");
        }

        [TestMethod]
        public void Reversibility()
        {
            var cryptographer = getCryptographer();

            var encrypted = cryptographer.Encrypt(SampleString);
            var decrypted = cryptographer.Decrypt(encrypted);

            Assert.AreEqual(SampleString, decrypted, "Decrypted value should match original.");
        }

        public ICryptographer getCryptographer()
        {
            return new Cryptographer();
        }
    }
}
