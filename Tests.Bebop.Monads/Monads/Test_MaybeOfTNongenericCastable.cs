// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using NUnit.Framework;

namespace Bebop.Monads
{
    [TestFixture]
    public class Test_MaybeOfTNongenericCastable
    {
        [TestFixture]
        public class Construction
        {
            [Test]
            public void CanConstructEmpty()
            {
                var m = Maybe.Castable.Nothing(typeof(int));

                Assert.IsFalse(((IMaybe) m).HasValue);
                Assert.IsInstanceOf<Maybe<int>>(m);
            }

            [Test]
            public void CanConstructWithValue()
            {
                var m = Maybe.Castable.From(typeof(int), 123);

                Assert.IsTrue(((IMaybe) m).HasValue);
                Assert.AreEqual(123, ((IMaybe) m).Value);
                Assert.IsInstanceOf<Maybe<int>>(m);
            }

            [Test]
            public void CanConstructEmpty_RejectsInvalidArguments()
            {
                Assert.Throws<ArgumentNullException>(() => Maybe.Castable.Nothing(null));
            }

            [Test]
            public void CanConstructWithValue_RejectsInvalidArguments()
            {
                Assert.Throws<ArgumentNullException>(() => Maybe.Castable.From(null, 123));
                Assert.Throws<ArgumentNullException>(() => Maybe.Castable.From(typeof(int), null));
                Assert.Throws<ArgumentException>(() => Maybe.Castable.From(typeof(int), Guid.Empty));
            }
        }
        
        [TestFixture]
        public class Equality
        {
            [Test]
            public void ProvidesHashCode()
            {
                var a = Maybe.Castable.From(typeof(int), 123);
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

                var a = Maybe.Castable.From(typeof(Mutable), o);
                Assert.AreEqual(-999, a.GetHashCode());

                o._hashCode = 777;
                Assert.AreEqual(777, a.GetHashCode());
            }

            [Test]
            public void ProvidesHashCode_Nothing()
            {
                var a = Maybe.Castable.Nothing(typeof(int));
                Assert.AreEqual(typeof(int).GetHashCode(), a.GetHashCode());
            }

            [Test]
            public void Equality_MaybeOfT()
            {
                var a = Maybe.Castable.From(typeof(int), 123);
                var b = Maybe.Castable.From(typeof(int), 123);
                var c = Maybe.Castable.From(typeof(int), 123);
                var d = Maybe.Castable.From(typeof(int), 456);
                var e = Maybe.Castable.From(typeof(string), "pretzels");
                var f = Maybe.Castable.Nothing(typeof(int));
                var g = Maybe.Castable.Nothing(typeof(string));

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
                IMaybe a = Maybe.Castable.From(typeof(int),123);
                IMaybe b = Maybe.Castable.From(typeof(int),123);
                IMaybe c = Maybe.Castable.From(typeof(int),123);
                IMaybe d = Maybe.Castable.From(typeof(int),456);
                IMaybe e = Maybe.Castable.From(typeof(string), "pretzels");
                IMaybe f = Maybe.Castable.Nothing(typeof(int));
                IMaybe g = Maybe.Castable.Nothing(typeof(string));

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
                object a = Maybe.Castable.From(typeof(int),123);
                object b = Maybe.Castable.From(typeof(int),123);
                object c = Maybe.Castable.From(typeof(int),123);
                object d = Maybe.Castable.From(typeof(int),456);
                object e = Maybe.Castable.From(typeof(string), "pretzels");
                object f = Maybe.Castable.Nothing(typeof(int));
                object g = Maybe.Castable.Nothing(typeof(string));

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
            public void InterfaceHierarchy()
            {
                var m1 = Maybe.Castable.From(typeof(int), 123);
                var m2 = Maybe.Castable.Nothing(typeof(int));

                Assert.IsInstanceOf<IMaybe>(m1);
                Assert.IsInstanceOf<IMaybe>(m2);
            }

            [Test]
            public void ProvidesTypeInformation()
            {
                var m = Maybe.Castable.From(typeof(int), 123);
                var n = Maybe.Castable.From(typeof(string), "yada yada yada");

                var o = Maybe.Castable.Nothing(typeof(int));
                var p = Maybe.Castable.Nothing(typeof(string));

                Assert.AreEqual(typeof(int), ((IMaybe) m).InternalType);
                Assert.AreEqual(typeof(string), ((IMaybe) n).InternalType);

                Assert.AreEqual(typeof(int), ((IMaybe) o).InternalType);
                Assert.AreEqual(typeof(string), ((IMaybe) p).InternalType);
            }

            [Test]
            public void ProvidesValueInformation()
            {
                var m = Maybe.Castable.From(typeof(int), 123);
                var n = Maybe.Castable.Nothing(typeof(int));

                Assert.IsTrue(((IMaybe) m).HasValue);
                Assert.IsFalse(((IMaybe) n).HasValue);
            }

            [Test]
            public void CanGetValue_Object_ViaProperty()
            {
                IMaybe m = Maybe.Castable.From(typeof(int), 123);
                IMaybe n = Maybe.Castable.From(typeof(string), "yada yada yada");

                var o = Maybe.Castable.Nothing(typeof(int));
                var p = Maybe.Castable.Nothing(typeof(string));

                Assert.IsInstanceOf<int>(m.Value);
                Assert.IsInstanceOf<string>(n.Value);

                Assert.AreEqual(123, m.Value);
                Assert.AreEqual("yada yada yada", n.Value);

                Assert.Throws<InvalidOperationException>(() => { var x = ((IMaybe)o).Value; });
                Assert.Throws<InvalidOperationException>(() => { var x = ((IMaybe)p).Value; });
            }

            [Test]
            public void ProvidesDebuggerDisplay()
            {
                var m = (Maybe<string>) Maybe.Castable.From(typeof(string), "yada yada yada");
                var n = (Maybe<int>) Maybe.Castable.Nothing(typeof(int));

                Assert.AreEqual("yada yada yada", m.DebuggerDisplay);
                Assert.AreEqual("Nothing<Int32>", n.DebuggerDisplay);
            }

            [Test]
            public void ProvidesStringRepresentation()
            {
                var m = Maybe.Castable.From(typeof(string), "yada yada yada");
                var n = Maybe.Castable.Nothing(typeof(int));

                Assert.AreEqual("yada yada yada", m.ToString());
                Assert.AreEqual("Nothing<Int32>", n.ToString());
            }
        }
    }
}
