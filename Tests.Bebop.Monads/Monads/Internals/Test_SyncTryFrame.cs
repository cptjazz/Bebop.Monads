using System;
using NUnit.Framework;

namespace Bebop.Monads.Internals
{
    internal sealed class Test_SyncTryFrame
    {
        [Test]
        public void CanCreate()
        {
            var type = TryFrameType.CatchClause;
            Func<object, object> action = o => o;
            var clause = new SyncCatchClause(exception => 9, typeof(ArithmeticException));

            var x = new SyncTryFrame(type, action, clause);

            Assert.AreEqual(type, x.FrameType);
            Assert.AreSame(action, x.Action);
            Assert.AreSame(clause, x.CatchClause);
        }
    }
}
