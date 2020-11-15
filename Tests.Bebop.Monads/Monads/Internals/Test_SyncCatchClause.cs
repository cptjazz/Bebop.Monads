// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Bebop.Monads.Internals
{
    [TestFixture]
    internal sealed class Test_SyncCatchClause
    {
        [Test]
        public void CanCreate()
        {
            Func<Exception, object> handler = exception => 9;
            var type = typeof(ArithmeticException);

            var x = new SyncCatchClause(handler, type);

            Assert.AreSame(handler, x.Handler);
            Assert.AreEqual(type, x.ExceptionType);
        }

        [Test]
        public void CanDetermineHandlerCompatibility()
        {
            Func<Exception, object> handler = exception => 9;
            var type = typeof(OperationCanceledException);

            var x = new SyncCatchClause(handler, type);

            Assert.IsTrue(x.CanHandle(new OperationCanceledException()));
            Assert.IsTrue(x.CanHandle(new TaskCanceledException()));
            Assert.IsFalse(x.CanHandle(new ArithmeticException()));
            Assert.IsFalse(x.CanHandle(new ArgumentNullException()));
        }

        [Test]
        public async Task CanConvertToAsync()
        {
            var wasCalled = false;
            Func<Exception, object> handler = exception =>
            {
                wasCalled = true;
                return 9;
            };

            var type = typeof(ArithmeticException);

            var x = new SyncCatchClause(handler, type);
            var y = x.ToAsync();

            Assert.AreEqual(type, y.ExceptionType);
            Assert.IsFalse(wasCalled);

            await y.Handler(new ArithmeticException());
            Assert.IsTrue(wasCalled);
        }
    }
}
