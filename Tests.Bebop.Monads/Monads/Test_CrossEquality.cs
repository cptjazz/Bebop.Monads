// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using NUnit.Framework;

namespace Bebop.Monads
{
    [TestFixture]
    public class Test_CrossEquality
    {
        [Test]
        public void ReferenceType()
        {
            var a = Maybe.From("yada");
            var b = Maybe.From(typeof(string), "yada");

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals(b));

            Assert.IsTrue(b.Equals(a));
            Assert.IsTrue(b.Equals(b));
        }

        [Test]
        public void ValueType()
        {
            var a = Maybe.From(182);
            var b = Maybe.From(typeof(int), 182);

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals(b));
                     
            Assert.IsTrue(b.Equals(a));
            Assert.IsTrue(b.Equals(b));
        }

        [Test]
        public void TypeMismatch()
        {
            var a = Maybe.From("yada");
            var b = Maybe.From(typeof(int), 182);

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(b.Equals(a));
        }

        [Test]
        public void ValueTypeDefaultVsNothing()
        {
            var a = Maybe.From(0);
            var b = Maybe.Nothing<int>();
            var c = Maybe.From(typeof(int), 0);
            var d = Maybe.Nothing(typeof(int));

            Assert.IsTrue(a.Equals(a));
            Assert.IsFalse(a.Equals(b));
            Assert.IsTrue(a.Equals(c));
            Assert.IsFalse(a.Equals(d));
                           
            Assert.IsFalse(b.Equals(a));
            Assert.IsTrue(b.Equals(b));
            Assert.IsFalse(b.Equals(c));
            Assert.IsTrue(b.Equals(d));
                           
            Assert.IsTrue(c.Equals(a));
            Assert.IsFalse(c.Equals(b));
            Assert.IsTrue(c.Equals(c));
            Assert.IsFalse(c.Equals(d));
                           
            Assert.IsFalse(d.Equals(a));
            Assert.IsTrue(d.Equals(b));
            Assert.IsFalse(d.Equals(c));
            Assert.IsTrue(d.Equals(d));
        }
    }
}