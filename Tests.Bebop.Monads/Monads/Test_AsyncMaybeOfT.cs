// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using NUnit.Framework;
using System;
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
            var r = await m.AsValueTask();

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
        public void ProvidesTypeInformation()
        {
            var m = new AsyncMaybe<int>();
            Assert.AreEqual(typeof(int), ((IAsyncMaybe)m).InternalType);
        }

        [Test]
        public void RejectsInvalidArguments()
        {
            var m = new AsyncMaybe<int>();

            Assert.Throws<ArgumentNullException>(() => m.MapAsync<string>(null));
            Assert.Throws<ArgumentNullException>(() => m.Map<string>(null));
        }

        [TestFixture]
        public class OnAsyncMaybe
        {
            [TestFixture]
            public class AsTask
            {
                [Test]
                public async Task FromMapFromMapFrom()
                {
                    var result = await Maybe
                        .From(55)
                        .MapAsync(async x => Maybe.From(66))
                        .Map(x => Maybe.From(77))
                        .AsValueTask();

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
                        .Map(x =>
                        {
                            Assert.Fail("should not be called");
                            return Maybe.From(77);
                        })
                        .AsValueTask();

                    Assert.AreEqual(Maybe.Nothing<int>(), result);
                }

                [Test]
                public async Task FromMapNothingMapFrom()
                {
                    var result = await Maybe
                        .From(55)
                        .MapAsync(async x => Maybe.Nothing<int>())
                        .Map(x =>
                        {
                            Assert.Fail("should not be called");
                            return Maybe.From(77);
                        })
                        .AsValueTask();

                    Assert.AreEqual(Maybe.Nothing<int>(), result);
                }

                [Test]
                public async Task FromMapFromMapNothing()
                {
                    var result = await Maybe
                        .From(55)
                        .MapAsync(async x => Maybe.From(66))
                        .Map(x => Maybe.Nothing<int>())
                        .AsValueTask();

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
                        .Map(x => Maybe.From(77))
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
                        .Map(x =>
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
                        .Map(x =>
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
                        .Map(x => Maybe.Nothing<int>())
                        .OrElse(99);

                    Assert.AreEqual(99, result);
                }
            }
        }

        [TestFixture]
        public class OnIAsyncMaybe
        {
            [TestFixture]
            public class AsTask
            {
                [Test]
                public async Task FromMapFromMapFrom()
                {
                    var result = await ((IMaybe<int>) Maybe
                        .From(55))
                        .MapAsync(async x => Maybe.From(66))
                        .Map(x => Maybe.From(77))
                        .AsTask();

                    Assert.AreEqual(Maybe.From(77), result);
                }

                [Test]
                public async Task NothingMapFromMapFrom()
                {
                    var result = await ((IMaybe<int>) Maybe
                        .Nothing<int>())
                        .MapAsync(async x =>
                        {
                            Assert.Fail("should not be called");
                            return Maybe.From(66);
                        })
                        .Map(x =>
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
                    var result = await ((IMaybe<int>)Maybe
                        .From(55))
                        .MapAsync(async x => Maybe.Nothing<int>())
                        .Map(x =>
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
                    var result = await ((IMaybe<int>)Maybe
                        .From(55))
                        .MapAsync(async x => Maybe.From(66))
                        .Map(x => Maybe.Nothing<int>())
                        .AsTask();

                    Assert.AreEqual(Maybe.Nothing<int>(), result);
                }
            }
        }
    }
}
