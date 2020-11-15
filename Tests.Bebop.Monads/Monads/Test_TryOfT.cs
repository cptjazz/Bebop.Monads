// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using NUnit.Framework;
using System;
using System.Globalization;
using System.Threading.Tasks;

#pragma warning disable CS1998

namespace Bebop.Monads
{
    [TestFixture]
    public class Test_TryOfT
    {
        [TestFixture]
        public class Default
        {
            [Test]
            public void ExecuteEmptyTry()
            {
                var result = default(Try<int>)
                    .Execute();

                Assert.AreEqual(default(int), result);
            }

            [Test]
            public void ExecuteEmptyTryWithThen()
            {
                var result = default(Try<int>)
                    .Then(() => 99)
                    .Catch<ArithmeticException>(_ => 9)
                    .Execute();

                Assert.AreEqual(99, result);
            }

            [Test]
            public void ExecuteEmptyTryWithThenWithCaughtException()
            {
                var result = default(Try<int>)
                    .Then<string>(() => throw new ArithmeticException())
                    .Catch<ArithmeticException>(_ => "9")
                    .Execute();

                Assert.AreEqual("9", result);
            }

            [Test]
            public void ExecuteEmptyTryWithThenWithUncaughtException()
            {
                var t = default(Try<int>)
                    .Then<string>(() => throw new ArithmeticException())
                    .Catch<ArgumentException>(_ => "9");

                Assert.Throws<ArithmeticException>(() => t.Execute());
            }

            [Test]
            public void ExecuteEmptyTryWithCatch()
            {
                var result = default(Try<int>)
                    .Catch<ArithmeticException>(_ => 9)
                    .Execute();

                Assert.AreEqual(default(int), result);
            }
        }

        [TestFixture]
        public class DefaultAwaited
        {
            [Test]
            public async Task ExecuteEmptyTry()
            {
                var result = await default(Try<int>);

                Assert.AreEqual(default(int), result);
            }

            [Test]
            public async Task ExecuteEmptyTryWithThen()
            {
                var result = await default(Try<int>)
                    .Then(() => 99)
                    .Catch<ArithmeticException>(_ => 9);

                Assert.AreEqual(99, result);
            }

            [Test]
            public async Task ExecuteEmptyTryWithThenWithCaughtException()
            {
                var result = await default(Try<int>)
                    .Then<string>(() => throw new ArithmeticException())
                    .Catch<ArithmeticException>(_ => "9");

                Assert.AreEqual("9", result);
            }

            [Test]
            public void ExecuteEmptyTryWithThenWithUncaughtException()
            {
                var t = default(Try<int>)
                    .Then<string>(() => throw new ArithmeticException())
                    .Catch<ArgumentException>(_ => "9");

                Assert.ThrowsAsync<ArithmeticException>(async () => await t);
            }

            [Test]
            public async Task ExecuteEmptyTryWithCatch()
            {
                var result = await default(Try<int>)
                    .Catch<ArithmeticException>(_ => 9);

                Assert.AreEqual(default(int), result);
            }
        }


        [TestFixture]
        public class FromFunc
        {
            [Test]
            public void ThenCatch()
            {
                var result = Try
                    .Do(() => 99)
                    .Then(x => x.ToString(CultureInfo.InvariantCulture))
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .Execute();

                Assert.AreEqual("99", result);
            }

            [Test]
            public void Then2Catch()
            {
                var result = Try
                    .Do(() => 99)
                    .Then(() => "99")
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .Execute();

                Assert.AreEqual("99", result);
            }

            [Test]
            public void ThenCatchNongeneric()
            {
                var result = Try
                    .Do(() => 99)
                    .Then(x => x.ToString(CultureInfo.InvariantCulture))
                    .Catch(typeof(ArithmeticException), ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .Execute();

                Assert.AreEqual("99", result);
            }

            [Test]
            public async Task ThenAsyncCatch()
            {
                var result = await Try
                    .Do(() => 99)
                    .ThenAsync(async x => x.ToString(CultureInfo.InvariantCulture))
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .ExecuteAsync();

                Assert.AreEqual("99", result);
            }

            [Test]
            public async Task ThenAsync2Catch()
            {
                var result = await Try
                    .Do(() => 99)
                    .ThenAsync(async () => "99")
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .ExecuteAsync();

                Assert.AreEqual("99", result);
            }

            [Test]
            public async Task ThenCatchAsync()
            {
                var result = await Try
                    .Do(() => 99)
                    .Then(x => x.ToString(CultureInfo.InvariantCulture))
                    .CatchAsync<ArithmeticException>(async ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .ExecuteAsync();

                Assert.AreEqual("99", result);
            }

            [Test]
            public async Task ThenCatchAsyncNongeneric()
            {
                var result = await Try
                    .Do(() => 99)
                    .Then(x => x.ToString(CultureInfo.InvariantCulture))
                    .CatchAsync(typeof(ArithmeticException), async ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .ExecuteAsync();

                Assert.AreEqual("99", result);
            }

            [Test]
            public async Task ThenAsyncCatchAsync()
            {
                var result = await Try
                    .Do(() => 99)
                    .ThenAsync(async x => x.ToString(CultureInfo.InvariantCulture))
                    .CatchAsync<ArithmeticException>(async ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .ExecuteAsync();

                Assert.AreEqual("99", result);
            }
        }

        [TestFixture]
        public class FromFunc_Throwing
        {
            [Test]
            public void ThenCatch()
            {
                var wasCalled = false;
                var result = Try
                    .Do(() => 99)
                    .Then<string>(x => throw new ArithmeticException())
                    .Catch<ArithmeticException>(ex =>
                    {
                        wasCalled = true;
                        return "9";
                    })
                    .Execute();

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public void ThenCatchNongeneric()
            {
                var wasCalled = false;
                var result = Try
                    .Do(() => 99)
                    .Then<string>(x => throw new ArithmeticException())
                    .Catch(typeof(ArithmeticException), ex =>
                    {
                        wasCalled = true;
                        return "9";
                    })
                    .Execute();

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenAsyncCatch()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .ThenAsync<string>(async x => throw new ArithmeticException())
                    .Catch<ArithmeticException>(ex =>
                    {
                        wasCalled = true;
                        return "9";
                    })
                    .ExecuteAsync();

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenCatchAsync()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .Then<string>(x => throw new ArithmeticException())
                    .CatchAsync<ArithmeticException>(async ex =>
                    {
                        wasCalled = true;
                        return "9";
                    })
                    .ExecuteAsync();

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenCatchAsyncNongeneric()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .Then<string>(x => throw new ArithmeticException())
                    .CatchAsync(typeof(ArithmeticException), async ex =>
                    {
                        wasCalled = true;
                        return "9";
                    })
                    .ExecuteAsync();

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenAsyncCatchAsync()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .ThenAsync<string>(async x => throw new ArithmeticException())
                    .CatchAsync<ArithmeticException>(async ex =>
                    {
                        wasCalled = true;
                        return "9";
                    })
                    .ExecuteAsync();

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }
        }

        [TestFixture]
        public class FromFuncAwaited
        {
            [Test]
            public async Task ThenCatch()
            {
                var result = await Try
                    .Do(() => 99)
                    .Then(x => x.ToString(CultureInfo.InvariantCulture))
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    });

                Assert.AreEqual("99", result);
            }

            [Test]
            public async Task ThenCatchNongeneric()
            {
                var result = await Try
                    .Do(() => 99)
                    .Then(x => x.ToString(CultureInfo.InvariantCulture))
                    .Catch(typeof(ArithmeticException), ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    });

                Assert.AreEqual("99", result);
            }

            [Test]
            public async Task ThenAsyncCatch()
            {
                var result = await Try
                    .Do(() => 99)
                    .ThenAsync(async x => x.ToString(CultureInfo.InvariantCulture))
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    });

                Assert.AreEqual("99", result);
            }

            [Test]
            public async Task ThenCatchAsync()
            {
                var result = await Try
                    .Do(() => 99)
                    .Then(x => x.ToString(CultureInfo.InvariantCulture))
                    .CatchAsync<ArithmeticException>(async ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    });

                Assert.AreEqual("99", result);
            }

            [Test]
            public async Task ThenCatchAsyncNongeneric()
            {
                var result = await Try
                    .Do(() => 99)
                    .Then(x => x.ToString(CultureInfo.InvariantCulture))
                    .CatchAsync(typeof(ArithmeticException), async ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    });

                Assert.AreEqual("99", result);
            }

            [Test]
            public async Task ThenAsyncCatchAsync()
            {
                var result = await Try
                    .Do(() => 99)
                    .ThenAsync(async x => x.ToString(CultureInfo.InvariantCulture))
                    .CatchAsync<ArithmeticException>(async ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    });

                Assert.AreEqual("99", result);
            }
        }

        [TestFixture]
        public class FromFuncAwaited_Throwing
        {
            [Test]
            public async Task ThenCatch()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .Then<string>(x => throw new ArithmeticException())
                    .Catch<ArithmeticException>(ex =>
                    {
                        wasCalled = true;
                        return "9";
                    });

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenCatchNongeneric()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .Then<string>(x => throw new ArithmeticException())
                    .Catch(typeof(ArithmeticException), ex =>
                    {
                        wasCalled = true;
                        return "9";
                    });

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenAsyncCatch()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .ThenAsync<string>(async x => throw new ArithmeticException())
                    .Catch<ArithmeticException>(ex =>
                    {
                        wasCalled = true;
                        return "9";
                    });

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenCatchAsync()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .Then<string>(x => throw new ArithmeticException())
                    .CatchAsync<ArithmeticException>(async ex =>
                    {
                        wasCalled = true;
                        return "9";
                    });

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenCatchAsyncNongeneric()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .Then<string>(x => throw new ArithmeticException())
                    .CatchAsync(typeof(ArithmeticException), async ex =>
                    {
                        wasCalled = true;
                        return "9";
                    });

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenAsyncCatchAsync()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .ThenAsync<string>(async x => throw new ArithmeticException())
                    .CatchAsync<ArithmeticException>(async ex =>
                    {
                        wasCalled = true;
                        return "9";
                    });

                Assert.AreEqual("9", result);
                Assert.IsTrue(wasCalled);
            }
        }

        [TestFixture]
        public class MultipleCatchClauses
        {
            [Test]
            public async Task ThenThenCatchThenCatch()
            {
                var result = await Try
                    .Do(() => 99)
                    .Then(x => x.ToString(CultureInfo.InvariantCulture))
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .Then(x => TimeSpan.FromSeconds(int.Parse(x)))
                    .Catch<ArgumentException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return TimeSpan.FromMinutes(12);
                    });

                Assert.AreEqual(TimeSpan.FromSeconds(99), result);
            }

            [Test]
            public async Task ThenThenAsyncCatchThenCatchAsync()
            {
                var result = await Try
                    .Do(() => 99)
                    .ThenAsync(async x => x.ToString(CultureInfo.InvariantCulture))
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .Then(x => TimeSpan.FromSeconds(int.Parse(x)))
                    .CatchAsync<ArgumentException>(async ex =>
                    {
                        Assert.Fail("should not happen");
                        return TimeSpan.FromMinutes(12);
                    });

                Assert.AreEqual(TimeSpan.FromSeconds(99), result);
            }
        }

        [TestFixture]
        public class MultipleCatchClauses_Throwing
        {
            [Test]
            public async Task ThenThenCatchThenCatch_ThrowOn1_Caught1()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .Then<string>(x => throw new ArithmeticException())
                    .Catch<ArithmeticException>(ex =>
                    {
                        wasCalled = true;
                        return "9";
                    })
                    .Then(x => TimeSpan.FromSeconds(int.Parse(x)))
                    .Catch<ArgumentException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return TimeSpan.FromMinutes(12);
                    });

                Assert.AreEqual(TimeSpan.FromSeconds(9), result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenThenCatchThenCatch_ThrowOn1_Caught2()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .Then<string>(x => throw new ArgumentException())
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .Then(x => TimeSpan.FromSeconds(int.Parse(x)))
                    .Catch<ArgumentException>(ex =>
                    {
                        wasCalled = true;
                        return TimeSpan.FromMinutes(12);
                    });

                Assert.AreEqual(TimeSpan.FromMinutes(12), result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenThenCatchThenCatch_ThrowOn1_Uncaught()
            {
                var result = Try
                    .Do(() => 99)
                    .Then<string>(x => throw new InvalidOperationException())
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .Then(x => TimeSpan.FromSeconds(int.Parse(x)))
                    .Catch<ArgumentException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return TimeSpan.FromMinutes(12);
                    });

                Assert.ThrowsAsync<InvalidOperationException>(async () => await result);
            }

            [Test]
            public async Task ThenThenCatchThenCatch_ThrowOn2_Caught2()
            {
                var wasCalled = false;
                var result = await Try
                    .Do(() => 99)
                    .Then(x => x.ToString(CultureInfo.InvariantCulture))
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .Then<TimeSpan>(x => throw new ArgumentNullException())
                    .Catch<ArgumentException>(ex =>
                    {
                        wasCalled = true;
                        return TimeSpan.FromMinutes(12);
                    });

                Assert.AreEqual(TimeSpan.FromMinutes(12), result);
                Assert.IsTrue(wasCalled);
            }

            [Test]
            public async Task ThenThenCatchThenCatch_ThrowOn2_Uncaught()
            {
                var result = Try
                    .Do(() => 99)
                    .Then(x => x.ToString(CultureInfo.InvariantCulture))
                    .Catch<ArithmeticException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return "9";
                    })
                    .Then<TimeSpan>(x => throw new InvalidOperationException())
                    .Catch<ArgumentException>(ex =>
                    {
                        Assert.Fail("should not happen");
                        return TimeSpan.FromMinutes(12);
                    });

                Assert.ThrowsAsync<InvalidOperationException>(async () => await result);
            }
        }

        [TestFixture]
        public class Arguments
        {
            [Test]
            public void RejectsInvalidArguments_Then()
            {
                var t = Try.Do(() => 99);

                Assert.Throws<ArgumentNullException>(() => t.Then((Func<int, string>)null));
                Assert.Throws<ArgumentNullException>(() => t.Then((Func<string>)null));
            }

            [Test]
            public void RejectsInvalidArguments_ThenAsync()
            {
                var t = Try.Do(() => 99);

                Assert.Throws<ArgumentNullException>(() => t.ThenAsync((Func<int, Task<string>>)null));
                Assert.Throws<ArgumentNullException>(() => t.ThenAsync((Func<Task<string>>)null));
            }

            [Test]
            public void RejectsInvalidArguments_Catch()
            {
                var t = Try.Do(() => 99);

                Assert.Throws<ArgumentNullException>(() => t.Catch<ArithmeticException>(null));
                Assert.Throws<ArgumentNullException>(() => t.Catch(null, ex => 9));
                Assert.Throws<ArgumentNullException>(() => t.Catch(typeof(ArithmeticException), null));
                Assert.Throws<ArgumentException>(() => t.Catch(typeof(object), null));
            }

            [Test]
            public void RejectsInvalidArguments_CatchAsync()
            {
                var t = Try.Do(() => 99);

                Assert.Throws<ArgumentNullException>(() => t.CatchAsync<ArithmeticException>(null));
                Assert.Throws<ArgumentNullException>(() => t.CatchAsync(null, async ex => 9));
                Assert.Throws<ArgumentNullException>(() => t.CatchAsync(typeof(ArithmeticException), null));
                Assert.Throws<ArgumentException>(() => t.CatchAsync(typeof(object), null));
            }
        }
    }
}
