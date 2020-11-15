// Copyright 2019, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using NUnit.Framework;

namespace Bebop.Monads
{
    [TestFixture]
    public class Test_Unit
    {
        [Test]
        public void CanCreate()
        {
            var u = new Unit();
            var v = default(Unit);

            Assert.Pass();
        }

        [Test]
        public void ProvidesHashCode()
        {
            Assert.AreEqual(37, default(Unit).GetHashCode());
        }

        [Test]
        public void EqualityMembers()
        {
            var a = default(Unit);
            var b = default(Unit);
            var c = default(Unit);

            // reflexive
            Assert.IsTrue(a.Equals(a));

            // symmetric
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(a));

            // transitive
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));

            // others
            Assert.IsFalse(a.Equals(new object()));

            // null
            Assert.IsFalse(a.Equals((object) null));
        }

        [Test]
        public void EqualityMembersSharedInstance()
        {
            var a = Unit.Instance;
            var b = Unit.Instance;
            var c = Unit.Instance;

            // reflexive
            Assert.IsTrue(a.Equals(a));

            // symmetric
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(a));

            // transitive
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(c));
            Assert.IsTrue(a.Equals(c));

            // others
            Assert.IsFalse(a.Equals(new object()));

            // null
            Assert.IsFalse(a.Equals((IUnit) null));
        }

        [Test]
        public void EqualityMembersAcrossTypes()
        {
            var a = default(Unit);
            var b = Unit.Instance;

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(b.Equals(a));
        }

        [Test]
        public void ProvidesSharedInstance()
        {
            var a = default(Unit);
            var b = default(Unit);
            var c = default(Unit);

            Assert.AreNotSame(a, b);
            Assert.AreNotSame(a, c);

            var x = Unit.Instance;
            var y = Unit.Instance;
            var z = Unit.Instance;

            Assert.AreSame(x, y);
            Assert.AreSame(x, z);
        }
    }
}
