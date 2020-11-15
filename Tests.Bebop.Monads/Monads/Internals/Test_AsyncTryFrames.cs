// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Bebop.Monads.Internals
{
    internal sealed class Test_AsyncTryFrames
    {
        [Test]
        public void CanCreate()
        {
            var frames = new[]
            {
                new AsyncTryFrame(
                    TryFrameType.Action,
                    _ => Task.FromResult(new object()),
                    null),
                new AsyncTryFrame(
                    TryFrameType.CatchClause,
                    null,
                    new AsyncCatchClause(
                        async _ => 9,
                        typeof(ArithmeticException))),
            };

            var x = new AsyncTryFrames(frames);

            Assert.AreEqual(2, x.Length);

            Assert.AreSame(frames[0], x[0]);
            Assert.AreSame(frames[1], x[1]);
        }

        [Test]
        public void CanAdd()
        {
            var f1 = new AsyncTryFrame(
                TryFrameType.Action, 
                _ => Task.FromResult(new object()),
                null);
            
            var f2 = new AsyncTryFrame(
                TryFrameType.CatchClause, 
                null, 
                new AsyncCatchClause(
                    async _ => 9, 
                    typeof(ArithmeticException)));

            var frames = new[]
            {
                f1,
            };

            var x = new AsyncTryFrames(frames);
            var y = x.Add(f2);

            Assert.AreEqual(1, x.Length);
            Assert.AreEqual(2, y.Length);

            Assert.AreSame(f1, x[0]);
            Assert.AreSame(f1, y[0]);
            Assert.AreSame(f2, y[1]);
        }
    }
}
