using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Models;
using Yammer.Chat.WP.Converters;

namespace Yammer.Chat.WP.Tests.Converters
{
    [TestClass]
    public class BoolToVisibilityConverterTests
    {
        [TestMethod]
        public void invalid_values()
        {
            Assert.ThrowsException<ArgumentException>(() => this.Convert(null));
            Assert.ThrowsException<ArgumentException>(() => this.Convert(""));
            Assert.ThrowsException<ArgumentException>(() => this.Convert(true, "random string"));
        }

        [TestMethod]
        public void converts_true()
        {
            var actual = this.Convert(true);

            Assert.AreEqual(Visibility.Visible, actual);
        }

        [TestMethod]
        public void converts_false()
        {
            var actual = this.Convert(false);

            Assert.AreEqual(Visibility.Collapsed, actual);
        }

        [TestMethod]
        public void converts_true_inverse()
        {
            var actual = this.Convert(true, "I");

            Assert.AreEqual(Visibility.Visible, actual);
        }

        [TestMethod]
        public void converts_false_inverse()
        {
            var actual = this.Convert(false, "I");

            Assert.AreEqual(Visibility.Collapsed, actual);
        }

        private object Convert(object value, object parameter = null)
        {
            var converter = new BoolToVisibilityConverter();
            return converter.Convert(value, null, parameter, null);
        }
    }
}
