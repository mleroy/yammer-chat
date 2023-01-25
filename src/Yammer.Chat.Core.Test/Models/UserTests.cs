using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yammer.Chat.Core.Models;

namespace Yammer.Chat.Core.Test.Models
{
	[TestClass]
    public class UserTests
    {
	[TestMethod]
        public void full_name_generation()
        {
            var user = new User { FirstName = "first", LastName = "last" };

            user.UpdateFullName();

            Assert.AreEqual("first last", user.FullName);
        }

		[TestMethod]
        public void mugshot_id_replacement()
        {
			var template = "https://mug0.assets-yammer.com/mugshot/images/{width}x{height}/";

            var user = new User { MugshotTemplate = template + "qgX9wbwgHxdnw6CrWBhw2Fl15zLT025B" };

            user.UpdateMugshotTemplate("1");

            Assert.AreEqual(template + "1", user.MugshotTemplate);
        }
    }
}
