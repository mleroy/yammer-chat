using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Models;
using Yammer.Chat.WP.Converters;

namespace Yammer.Chat.WP.Tests.Converters
{
    [TestClass]
    public class MugshotTemplateConverterTests
    {
        [TestMethod]
        public void InvalidValues()
        {
            Assert.ThrowsException<ArgumentException>(() => this.Convert(null));
            Assert.ThrowsException<ArgumentException>(() => this.Convert(""));
        }

        [TestMethod]
        public void CustomValue()
        {
            var templateUrl = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/krCD1QCG5VxTcW-tdqBjBRLC4VVFzRxM";
            var expected = "https://mug0.assets-yammer.com/mugshot/images/16x16/krCD1QCG5VxTcW-tdqBjBRLC4VVFzRxM";

            // Note: the converter requests a WXGA resolution image, which 1.6x bigger.
            var actual = this.Convert(templateUrl, 10.0);

            Assert.AreEqual(expected, actual, "Custom value should be inserted in template");
        }

        [TestMethod]
        public void DefaultValue()
        {
            var templateUrl = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/krCD1QCG5VxTcW-tdqBjBRLC4VVFzRxM";
            var expected = "https://mug0.assets-yammer.com/mugshot/images/128x128/krCD1QCG5VxTcW-tdqBjBRLC4VVFzRxM";

            var actual = this.Convert(templateUrl);

            Assert.AreEqual(expected, actual, "Default value should be inserted in template");
        }

        [TestMethod]
        public void NotATemplateUrl()
        {
            var templateUrl = "https://mug0.assets-yammer.com/mugshot/images/krCD1QCG5VxTcW-tdqBjBRLC4VVFzRxM";

            var actual = this.Convert(templateUrl);

            Assert.AreEqual(templateUrl, actual, "Non-template urls should be returned as-is");
        }

        private object Convert(object value, object parameter = null)
        {
            var converter = new MugshotTemplateConverter();
            return converter.Convert(value, null, parameter, null);
        }
    }
}
