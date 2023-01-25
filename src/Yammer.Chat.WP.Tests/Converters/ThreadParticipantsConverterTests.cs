using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core;
using Yammer.Chat.Core.Models;
using Yammer.Chat.WP.Converters;

namespace Yammer.Chat.WP.Tests.Converters
{
    [TestClass]
    public class ThreadParticipantsConverterTests
    {
        [TestMethod]
        public void InvalidValues()
        {
            Assert.ThrowsException<ArgumentException>(() => this.Convert(null));
            Assert.ThrowsException<ArgumentException>(() => this.Convert("string"));
            Assert.ThrowsException<ArgumentException>(() => this.Convert(new List<User>()));
        }

        [TestMethod]
        public void SingleParticipantShowsFullName()
        {
            var participant = getParticipant(1);

            var actual = this.Convert(new List<User> { participant });

            Assert.AreEqual(participant.FullName, actual, "A single participant should show the full name");
        }

        [TestMethod]
        public void TwoParticipantsShowsFirstNames()
        {
            var participant1 = getParticipant(1);
            var participant2 = getParticipant(2);

            var expected = string.Format("{0}, {1}", participant1.FirstName, participant2.FirstName);

            var actual = this.Convert(new List<User> { participant1, participant2 });

            Assert.AreEqual(expected, actual, "Two participants should show first names only");
        }

        [TestMethod]
        public void ThreeParticipantsShowsFirstNames()
        {
            var participant1 = getParticipant(1);
            var participant2 = getParticipant(2);
            var participant3 = getParticipant(3);

            var expected = string.Format("{0}, {1}, {2}", participant1.FirstName, participant2.FirstName, participant3.FirstName);

            var actual = this.Convert(new List<User> { participant1, participant2, participant3 });

            Assert.AreEqual(expected, actual, "Three participants should show first names only");
        }

        [TestMethod]
        public void MoreThanThreeParticipantsShowsFirstThreeFirstNames()
        {
            var participant1 = getParticipant(1);
            var participant2 = getParticipant(2);
            var participant3 = getParticipant(3);
            var participant4 = getParticipant(4);

            var expected = string.Format("{0}, {1}, {2} +1", participant1.FirstName, participant2.FirstName, participant3.FirstName);

            var actual = this.Convert(new List<User> { participant1, participant2, participant3, participant4 });

            Assert.AreEqual(expected, actual, "More than three participants should show first three first names only");
        }

        private object Convert(object value)
        {
            var converter = new ThreadParticipantsConverter();
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
    }
}
