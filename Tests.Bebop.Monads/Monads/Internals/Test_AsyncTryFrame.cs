// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;
using NUnit.Framework;

#pragma warning disable CS1998

namespace Bebop.Monads.Internals
{
    internal sealed class Test_AsyncTryFrame
    {
        [Test]
        public void CanCreate()
        {
            var type = TryFrameType.CatchClause;
            Func<object, Task<object>> action = async o => o;
            var clause = new AsyncCatchClause(exception => Task.CompletedTask, typeof(ArithmeticException));

            var x = new AsyncTryFrame(type, action, clause);

            Assert.AreEqual(type, x.FrameType);
            Assert.AreSame(action, x.Action);
            Assert.AreSame(clause, x.CatchClause);
        }
    }
}