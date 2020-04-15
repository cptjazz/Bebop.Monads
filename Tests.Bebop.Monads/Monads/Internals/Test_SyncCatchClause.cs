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
            Action<Exception> handler = exception => { };
            var type = typeof(ArithmeticException);

            var x = new SyncCatchClause(handler, type);

            Assert.AreSame(handler, x.Handler);
            Assert.AreEqual(type, x.ExceptionType);
        }

        [Test]
        public void CanDetermineHandlerCompatibility()
        {
            Action<Exception> handler = exception => { };
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
            Action<Exception> handler = exception => { wasCalled = true; };

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