using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammer.Chat.Core.Test
{
    [TestClass]
    public class DisposableActionTests
    {
        [TestMethod]
        public void ActionExecutedOnDispose()
        {
            var called = false;

            using (new DisposableAction(() => called = true)) { }

            Assert.IsTrue(called, "Disposing a disposable action should execute the action");
        }
    }
}
