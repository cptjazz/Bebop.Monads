// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Bebop.Monads.Internals
{
    internal sealed class Test_SyncTryFrames
    {
        [Test]
        public void CanCreate()
        {
            var frames = new[]
            {
                new SyncTryFrame(
                    TryFrameType.Action,
                    _ => new object(),
                    null),
                new SyncTryFrame(
                    TryFrameType.CatchClause,
                    null,
                    new SyncCatchClause(
                        _ => 9,
                        typeof(ArithmeticException))),
            };

            var x = new SyncTryFrames(frames);

            Assert.AreEqual(2, x.Length);

            Assert.AreSame(frames[0], x[0]);
            Assert.AreSame(frames[1], x[1]);
        }

        [Test]
        public void CanAdd()
        {
            var f1 = new SyncTryFrame(
                TryFrameType.Action, 
                _ => new object(),
                null);
            
            var f2 = new SyncTryFrame(
                TryFrameType.CatchClause, 
                null, 
                new SyncCatchClause(
                    _ => 9, 
                    typeof(ArithmeticException)));

            var frames = new[]
            {
                f1,
            };

            var x = new SyncTryFrames(frames);
            var y = x.Add(f2);

            Assert.AreEqual(1, x.Length);
            Assert.AreEqual(2, y.Length);

            Assert.AreSame(f1, x[0]);
            Assert.AreSame(f1, y[0]);
            Assert.AreSame(f2, y[1]);
        }

        [Test]
        public async Task CanConvertToAsync()
        {
            var wasF2Called = false;

            var f1 = new SyncTryFrame(
                TryFrameType.Action,
                _ => "my expected value",
                null);

            var f2 = new SyncTryFrame(
                TryFrameType.CatchClause,
                null,
                new SyncCatchClause(
                    _ => { wasF2Called = true; return 9; },
                    typeof(ArithmeticException)));

            var frames = new[] {f1, f2};

            var x = new SyncTryFrames(frames);
            var y = x.ToAsync();

            Assert.AreEqual(2, y.Length);

            var y1 = y[0];
            Assert.AreEqual(f1.FrameType, y1.FrameType);
            Assert.IsNull(y1.CatchClause);
            Assert.AreEqual("my expected value", await y1.Action(new object()));

            var y2 = y[1];
            Assert.AreEqual(f2.FrameType, y2.FrameType);
            Assert.IsNull(y2.Action);
            
            Assert.AreEqual(typeof(ArithmeticException), y2.CatchClause.ExceptionType);
            await y2.CatchClause.Handler(new ArithmeticException());
            Assert.IsTrue(wasF2Called);
        }
    }
}
