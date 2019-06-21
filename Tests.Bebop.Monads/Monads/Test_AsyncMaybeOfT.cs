// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using NUnit.Framework;
using System.Threading.Tasks;

namespace Bebop.Monads
{
    [TestFixture]
    public class Test_AsyncMaybeOfT
    {
        [Test]
        public async Task DefaultIsNothing()
        {
            var m = new AsyncMaybe<int>();
            var r = await m.AsTask();

            Assert.AreEqual(Maybe.Nothing<int>(), r);
        }

        [Test]
        public async Task CanAwait()
        {
            var m = new AsyncMaybe<int>();
            var r = await m;

            Assert.AreEqual(Maybe.Nothing<int>(), r);
        }

        [Test]
        public async Task SupportsImplicitCast()
        {
            Task<Maybe<int>> _MockProducer() => Task.FromResult(Maybe.From(55));
            AsyncMaybe<int> m = _MockProducer();

            Assert.AreEqual(Maybe.From(55), await m);
        }

        [Test]
        public void ProvidesTypeInformation()
        {
            var m = new AsyncMaybe<int>();
            Assert.AreEqual(typeof(int), ((IAsyncMaybe) m).InternalType);
        }

        [TestFixture]
        public class AsTask
        {
            [Test]
            public async Task FromMapFromMapFrom()
            {
                var result = await Maybe
                    .From(55)
                    .MapAsync(async x => Maybe.From(66))
                    .MapAsync(async x => Maybe.From(77))
                    .AsTask();

                Assert.AreEqual(Maybe.From(77), result);
            }

            [Test]
            public async Task NothingMapFromMapFrom()
            {
                var result = await Maybe
                    .Nothing<int>()
                    .MapAsync(async x =>
                    {
                        Assert.Fail("should not be called");
                        return Maybe.From(66);
                    })
                    .MapAsync(async x =>
                    {
                        Assert.Fail("should not be called");
                        return Maybe.From(77);
                    })
                    .AsTask();

                Assert.AreEqual(Maybe.Nothing<int>(), result);
            }

            [Test]
            public async Task FromMapNothingMapFrom()
            {
                var result = await Maybe
                    .From(55)
                    .MapAsync(async x => Maybe.Nothing<int>())
                    .MapAsync(async x =>
                    {
                        Assert.Fail("should not be called");
                        return Maybe.From(77);
                    })
                    .AsTask();

                Assert.AreEqual(Maybe.Nothing<int>(), result);
            }

            [Test]
            public async Task FromMapFromMapNothing()
            {
                var result = await Maybe
                    .From(55)
                    .MapAsync(async x => Maybe.From(66))
                    .MapAsync(async x => Maybe.Nothing<int>())
                    .AsTask();

                Assert.AreEqual(Maybe.Nothing<int>(), result);
            }
        }

        [TestFixture]
        public class OrElse
        {
            [Test]
            public async Task FromMapFromMapFrom()
            {
                var result = await Maybe
                    .From(55)
                    .MapAsync(async x => Maybe.From(66))
                    .MapAsync(async x => Maybe.From(77))
                    .OrElse(99);

                Assert.AreEqual(77, result);
            }

            [Test]
            public async Task NothingMapFromMapFrom()
            {
                var result = await Maybe
                    .Nothing<int>()
                    .MapAsync(async x =>
                    {
                        Assert.Fail("should not be called");
                        return Maybe.From(66);
                    })
                    .MapAsync(async x =>
                    {
                        Assert.Fail("should not be called");
                        return Maybe.From(77);
                    })
                    .OrElse(99);

                Assert.AreEqual(99, result);
            }

            [Test]
            public async Task FromMapNothingMapFrom()
            {
                var result = await Maybe
                    .From(55)
                    .MapAsync(async x => Maybe.Nothing<int>())
                    .MapAsync(async x =>
                    {
                        Assert.Fail("should not be called");
                        return Maybe.From(77);
                    })
                    .OrElse(99);

                Assert.AreEqual(99, result);
            }

            [Test]
            public async Task FromMapFromMapNothing()
            {
                var result = await Maybe
                    .From(55)
                    .MapAsync(async x => Maybe.From(66))
                    .MapAsync(async x => Maybe.Nothing<int>())
                    .OrElse(99);

                Assert.AreEqual(99, result);
            }
        }
    }
}
