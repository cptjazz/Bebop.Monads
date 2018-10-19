// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using NUnit.Framework;

namespace Bebop.Monads
{
    [TestFixture]
    public class Test_MaybeOfTNongeneric
    {
        [TestFixture]
        public class Construction
        {
            [Test]
            public void CanConstructEmpty()
            {
                var m = Maybe.Nothing(typeof(int));

                Assert.IsFalse(((IMaybe) m).HasValue);
            }

            [Test]
            public void CanConstructWithValue()
            {
                var m = Maybe.From(typeof(int), 123);

                Assert.IsTrue(((IMaybe) m).HasValue);
                Assert.AreEqual(123, ((IMaybe) m).GetValueOrDefault());
            }

            [Test]
            public void CanConstructEmpty_RejectsInvalidArguments()
            {
                Assert.Throws<ArgumentNullException>(() => Maybe.Nothing(null));
            }

            [Test]
            public void CanConstructWithValue_RejectsInvalidArguments()
            {
                Assert.Throws<ArgumentNullException>(() => Maybe.From(null, 123));
                Assert.Throws<ArgumentNullException>(() => Maybe.From(typeof(int), null));
                Assert.Throws<ArgumentException>(() => Maybe.From(typeof(int), Guid.Empty));
            }

            private class A { }
            private class B : A { }

            [Test]
            public void Covarinance()
            {
                IMaybe<A> o = Maybe.From(new B());
                IMaybe<A> p = Maybe.Nothing<B>();

                Assert.IsInstanceOf<B>(o.GetValueOrDefault());
            }
        }
        
        [TestFixture]
        public class Equality
        {
            [Test]
            public void ProvidesHashCode()
            {
                var a = Maybe.From(typeof(int), 123);
                Assert.AreEqual(123.GetHashCode(), a.GetHashCode());
            }

            private class Mutable
            {
                public int _hashCode = -999;

                public override int GetHashCode() => _hashCode;
            }

            [Test]
            public void ProvidesHashCode_Mutation()
            {
                var o = new Mutable();

                var a = Maybe.From(typeof(Mutable), o);
                Assert.AreEqual(-999, a.GetHashCode());

                o._hashCode = 777;
                Assert.AreEqual(777, a.GetHashCode());
            }

            [Test]
            public void ProvidesHashCode_Nothing()
            {
                var a = Maybe.Nothing(typeof(int));
                Assert.AreEqual(typeof(int).GetHashCode(), a.GetHashCode());
            }

            [Test]
            public void Equality_MaybeOfT()
            {
                var a = Maybe.From(typeof(int), 123);
                var b = Maybe.From(typeof(int), 123);
                var c = Maybe.From(typeof(int), 123);
                var d = Maybe.From(typeof(int), 456);
                var e = Maybe.From(typeof(string), "pretzels");
                var f = Maybe.Nothing(typeof(int));
                var g = Maybe.Nothing(typeof(string));

                // reflexive
                Assert.IsTrue(a.Equals(a));

                // symmetric
                Assert.IsTrue(a.Equals(b));
                Assert.IsTrue(b.Equals(a));
                
                // reflexive
                Assert.IsTrue(a.Equals(b));
                Assert.IsTrue(b.Equals(c));
                Assert.IsTrue(a.Equals(c));

                // inequality
                Assert.IsFalse(a.Equals(d));
                Assert.IsFalse(a.Equals(e));
                Assert.IsFalse(a.Equals(f));
                Assert.IsFalse(a.Equals(g));

                // others
                Assert.IsFalse(a.Equals(null));
                Assert.IsFalse(a.Equals(new object()));
            }

            [Test]
            public void Equality_IMaybe()
            {
                IMaybe a = Maybe.From(typeof(int),123);
                IMaybe b = Maybe.From(typeof(int),123);
                IMaybe c = Maybe.From(typeof(int),123);
                IMaybe d = Maybe.From(typeof(int),456);
                IMaybe e = Maybe.From(typeof(string), "pretzels");
                IMaybe f = Maybe.Nothing(typeof(int));
                IMaybe g = Maybe.Nothing(typeof(string));

                // reflexive
                Assert.IsTrue(a.Equals(a));

                // symmetric
                Assert.IsTrue(a.Equals(b));
                Assert.IsTrue(b.Equals(a));

                // reflexive
                Assert.IsTrue(a.Equals(b));
                Assert.IsTrue(b.Equals(c));
                Assert.IsTrue(a.Equals(c));

                // inequality
                Assert.IsFalse(a.Equals(d));
                Assert.IsFalse(a.Equals(e));
                Assert.IsFalse(a.Equals(f));
                Assert.IsFalse(a.Equals(g));

                // others
                Assert.IsFalse(a.Equals(null));
                Assert.IsFalse(a.Equals(new object()));
            }

            [Test]
            public void Equality_Object()
            {
                object a = Maybe.From(typeof(int),123);
                object b = Maybe.From(typeof(int),123);
                object c = Maybe.From(typeof(int),123);
                object d = Maybe.From(typeof(int),456);
                object e = Maybe.From(typeof(string), "pretzels");
                object f = Maybe.Nothing(typeof(int));
                object g = Maybe.Nothing(typeof(string));

                // reflexive
                Assert.IsTrue(a.Equals(a));

                // symmetric
                Assert.IsTrue(a.Equals(b));
                Assert.IsTrue(b.Equals(a));

                // reflexive
                Assert.IsTrue(a.Equals(b));
                Assert.IsTrue(b.Equals(c));
                Assert.IsTrue(a.Equals(c));

                // inequality
                Assert.IsFalse(a.Equals(d));
                Assert.IsFalse(a.Equals(e));
                Assert.IsFalse(a.Equals(f));
                Assert.IsFalse(a.Equals(g));

                // others
                Assert.IsFalse(a.Equals(null));
                Assert.IsFalse(a.Equals(new object()));
            }
        }

        [TestFixture]
        public class Infrastructure
        {
            [Test]
            public void ProvidesTypeInformation()
            {
                var m = Maybe.From(typeof(int), 123);
                var n = Maybe.From(typeof(string), "yada yada yada");

                var o = Maybe.Nothing(typeof(int));
                var p = Maybe.Nothing(typeof(string));

                Assert.AreEqual(typeof(int), ((IMaybe) m).InternalType);
                Assert.AreEqual(typeof(string), ((IMaybe) n).InternalType);

                Assert.AreEqual(typeof(int), ((IMaybe) o).InternalType);
                Assert.AreEqual(typeof(string), ((IMaybe) p).InternalType);
            }

            [Test]
            public void ProvidesValueInformation()
            {
                var m = Maybe.From(typeof(int), 123);
                var n = Maybe.Nothing(typeof(int));

                Assert.IsTrue(((IMaybe) m).HasValue);
                Assert.IsFalse(((IMaybe) n).HasValue);
            }

            [Test]
            public void CanGetValue_Object()
            {
                IMaybe m = Maybe.From(typeof(int), 123);
                IMaybe n = Maybe.From(typeof(string), "yada yada yada");

                var o = Maybe.Nothing(typeof(int));
                var p = Maybe.Nothing(typeof(string));

                Assert.IsInstanceOf<int>(m.GetValueOrDefault());
                Assert.IsInstanceOf<string>(n.GetValueOrDefault());

                Assert.AreEqual(123, m.GetValueOrDefault());
                Assert.AreEqual("yada yada yada", n.GetValueOrDefault());
                
                Assert.AreEqual(default(int), ((IMaybe)o).GetValueOrDefault());
                Assert.AreEqual(default(string), ((IMaybe)p).GetValueOrDefault());
            }

            [Test]
            public void ProvidesDebuggerDisplay()
            {
                var m = (MaybeNongeneric) Maybe.From(typeof(string), "yada yada yada");
                var n = (MaybeNongeneric) Maybe.Nothing(typeof(int));

                Assert.AreEqual("yada yada yada", m.DebuggerDisplay);
                Assert.AreEqual("Nothing<Int32>", n.DebuggerDisplay);
            }

            [Test]
            public void ProvidesStringRepresentation()
            {
                var m = Maybe.From(typeof(string), "yada yada yada");
                var n = Maybe.Nothing(typeof(int));

                Assert.AreEqual("yada yada yada", m.ToString());
                Assert.AreEqual("Nothing<Int32>", n.ToString());
            }
        }
    }
}
