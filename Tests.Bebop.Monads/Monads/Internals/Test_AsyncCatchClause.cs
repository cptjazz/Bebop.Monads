// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Bebop.Monads.Internals
{
    [TestFixture]
    internal sealed class Test_AsyncCatchClause
    {
        [Test]
        public void CanCreate()
        {
            Func<Exception, Task<object>> handler = async exception => 9;
            var type = typeof(ArithmeticException);

            var x = new AsyncCatchClause(handler, type);

            Assert.AreSame(handler, x.Handler);
            Assert.AreEqual(type, x.ExceptionType);
        }

        [Test]
        public void CanDetermineHandlerCompatibility()
        {
            Func<Exception, Task<object>> handler = async exception => 9;
            var type = typeof(OperationCanceledException);

            var x = new AsyncCatchClause(handler, type);

            Assert.IsTrue(x.CanHandle(new OperationCanceledException()));
            Assert.IsTrue(x.CanHandle(new TaskCanceledException()));
            Assert.IsFalse(x.CanHandle(new ArithmeticException()));
            Assert.IsFalse(x.CanHandle(new ArgumentNullException()));
        }
    }
}
