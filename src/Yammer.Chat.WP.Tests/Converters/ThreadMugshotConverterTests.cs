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
    public class ThreadMugshotConverterTests
    {
        [TestMethod]
        public void InvalidValues()
        {
            Assert.ThrowsException<ArgumentException>(() => this.Convert(null));
            Assert.ThrowsException<ArgumentException>(() => this.Convert("string"));
            Assert.ThrowsException<ArgumentException>(() => this.Convert(new List<User>()));
        }

        [TestMethod]
        public void SingleParticipantShowsMugshot()
        {
            var participant = getParticipant(1);

            var actual = this.Convert(new List<User> { participant });

            Assert.AreEqual(participant.MugshotTemplate, actual, "Participant mugshot should be returned for single participant");
        }

        [TestMethod]
        public void MoreThanOneParticipantShowsGroupImage()
        {
            var participant1 = getParticipant(1);
            var participant2 = getParticipant(2);

            var actual = this.Convert(new List<User> { participant1, participant2 });

            Assert.AreEqual("/Assets/groups.png", actual, "Group image should be returned for more than one participant");
        }

        private object Convert(object value)
        {
            var converter = new ThreadMugshotConverter { MugshotTemplateConverter = new MugshotTemplateConverterMock() };
            return converter.Convert(value, null, null, null);
        }

        private User getParticipant(int index)
        {
            return new User
            {
                FirstName = "First" + index,
                FullName = string.Format("First{0} Last{0}", index)
            };
        }

        class MugshotTemplateConverterMock : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return value as string;
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
