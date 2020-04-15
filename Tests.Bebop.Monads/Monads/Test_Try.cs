// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using NUnit.Framework;
using System;
using System.Threading.Tasks;

#pragma warning disable CS1998

namespace Bebop.Monads
{
    [TestFixture]
    public class Test_Try
    {
        private async Task<int> _DataFaker1()
        {
            return 99;
        }

        private async Task<int> _DataFaker2()
        {
            throw new ArithmeticException();
        }

        private async Task<string> _DataFaker3()
        {
            return "glory to you and your house";
        }

        [Test]
        public async Task UseCases()
        {
            var result = await Try
                .ToAsync(_DataFaker1)
                .ThenAsync(_DataFaker2)
                .Catch<ArithmeticException>(e => { /* dont care */})
                .ThenAsync(_DataFaker3);

            Assert.AreEqual(Maybe.Nothing<string>(), result);
        }

        [Test]
        public void RejectsInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => Try.To((Func<int>) null));
            Assert.Throws<ArgumentNullException>(() => Try.ToAsync((Func<Task<int>>) null));
        }
    }
}
