﻿// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Globalization;
using System.Threading.Tasks;
using NUnit.Framework;

#pragma warning disable CS1998

namespace Bebop.Monads
{
    [TestFixture]
    public class Test_MaybeOfT
    {
        [TestFixture]
        public class Construction
        {
            [Test]
            public void CanConstructEmpty()
            {
                Maybe<int> m = default;

                Assert.IsFalse(((IMaybe) m).HasValue);
            }

            [Test]
            public void CanConstructWithValue()
            {
                Maybe<int> m = Maybe.From(123);

                Assert.IsTrue(((IMaybe) m).HasValue);
                Assert.AreEqual(123, ((IMaybe<int>) m).Value);
            }

            [Test]
            public void CanConstructWithValue_RejectsInvalidArguments()
            {
                Assert.Throws<ArgumentNullException>(() => Maybe.From<string>(null));
            }

            [Test]
            public void CanConstructFromNullable_NoValue()
            {
                var n = default(Nullable<int>);
                var m = Maybe.From(n);

                Assert.AreEqual(Maybe.Nothing<int>(), m);
            }

            [Test]
            public void CanConstructFromNullable_HasValue()
            {
                var n = (Nullable<int>) 77;
                var m = Maybe.From(n);

                Assert.AreEqual(Maybe.From(77), m);
            }

            private Maybe<string> _ImplicitReceiverReferenceType(Maybe<string> m)
            {
                return m;
            }

            private Maybe<int> _ImplicitReceiverValueType(Maybe<int> m)
            {
                return m;
            }

            private Maybe<T> _ImplicitReceiverGeneric<T>(Maybe<T> m)
            {
                return m;
            }

            [Test]
            public void CanImplicitlyConvert()
            {
                Assert.AreEqual(Maybe.From("yada"), _ImplicitReceiverReferenceType("yada"));
                Assert.AreEqual(Maybe.From(182), _ImplicitReceiverValueType(182));
                Assert.AreEqual(Maybe.From("yada"), _ImplicitReceiverGeneric<string>("yada"));
                Assert.AreEqual(Maybe.From(182), _ImplicitReceiverGeneric<int>(182));
            }

            [Test]
            public void CanImplicitlyConvert_Nothing()
            {
                Assert.AreEqual(Maybe.Nothing<string>(), _ImplicitReceiverReferenceType(null));
                Assert.AreEqual(Maybe.Nothing<string>(), _ImplicitReceiverGeneric<string>(null));
            }
        }

        [TestFixture]
        public class Binding
        {
            [Test]
            public void CanMap()
            {
                var m = Maybe.From(123);
                var n = m.Map(x => Maybe.From(x.ToString(CultureInfo.InvariantCulture)));

                Assert.IsTrue(((IMaybe) n).HasValue);
                Assert.AreEqual("123", ((IMaybe<string>) n).Value);
            }

            [Test]
            public void CanMapToNothing()
            {
                var m = Maybe.From(123);
                var n = m.Map(x => Maybe.Nothing<string>());

                Assert.IsFalse(((IMaybe) n).HasValue);
                Assert.AreEqual(typeof(string), ((IMaybe) n).InternalType);
            }

            [Test]
            public void CanMapNothing()
            {
                var m = Maybe.Nothing<int>();
                var n = m.Map(x => Maybe.From(x.ToString(CultureInfo.InvariantCulture)));

                Assert.IsFalse(((IMaybe) n).HasValue);
                Assert.AreEqual(typeof(string), ((IMaybe) n).InternalType);
            }

            [Test]
            public void RejectsNullBinder()
            {
                var m = Maybe.From(123);

                Assert.Throws<ArgumentNullException>(() => m.Map<string>(null));
            }
        }
        
        [TestFixture]
        public class AsyncBinding
        {
            [Test]
            public async Task CanMap()
            {
                var m = Maybe.From(123);
                var n = await m.MapAsync(async x => Maybe.From(x.ToString(CultureInfo.InvariantCulture)));

                Assert.IsTrue(((IMaybe)n).HasValue);
                Assert.AreEqual("123", ((IMaybe<string>)n).Value);
            }

            [Test]
            public async Task CanMapToNothing()
            {
                var m = Maybe.From(123);
                var n = await m.MapAsync(async x => Maybe.Nothing<string>());

                Assert.IsFalse(((IMaybe)n).HasValue);
                Assert.AreEqual(typeof(string), ((IMaybe)n).InternalType);
            }

            [Test]
            public async Task CanMapNothing()
            {
                var m = Maybe.Nothing<int>();
                var n = await m.MapAsync(async x => Maybe.From(x.ToString(CultureInfo.InvariantCulture)));

                Assert.IsFalse(((IMaybe)n).HasValue);
                Assert.AreEqual(typeof(string), ((IMaybe)n).InternalType);
            }

            [Test]
            public async Task RejectsNullBinder()
            {
                var m = Maybe.From(123);

                Assert.ThrowsAsync<ArgumentNullException>(async () => await m.MapAsync<string>(null));
            }
        }

        [TestFixture]
        public class BindingViaInterface
        {
            [Test]
            public void CanMap()
            {
                var m = Maybe.From(123);
                var n = ((IMaybe<int>) m).Map(x => Maybe.From(x.ToString(CultureInfo.InvariantCulture)));

                Assert.IsTrue(((IMaybe) n).HasValue);
                Assert.AreEqual("123", ((IMaybe<string>) n).Value);
            }

            [Test]
            public void CanMapToNothing()
            {
                var m = Maybe.From(123);
                var n = ((IMaybe<int>)m).Map(x => Maybe.Nothing<string>());

                Assert.IsFalse(((IMaybe) n).HasValue);
                Assert.AreEqual(typeof(string), ((IMaybe) n).InternalType);
            }

            [Test]
            public void CanMapNothing()
            {
                var m = Maybe.Nothing<int>();
                var n = ((IMaybe<int>)m).Map(x => Maybe.From(x.ToString(CultureInfo.InvariantCulture)));

                Assert.IsFalse(((IMaybe) n).HasValue);
                Assert.AreEqual(typeof(string), ((IMaybe) n).InternalType);
            }

            [Test]
            public void RejectsNullBinder()
            {
                var m = Maybe.From(123);

                Assert.Throws<ArgumentNullException>(() => ((IMaybe<int>)m).Map<string>((Func<int, Maybe<string>>) null));
            }
        }
        
        [TestFixture]
        public class AsyncBindingViaInterface
        {
            [Test]
            public async Task CanMap()
            {
                var m = Maybe.From(123);
                var n = await ((IMaybe<int>)m).MapAsync<string>(async x => Maybe.From(x.ToString(CultureInfo.InvariantCulture)));

                Assert.IsTrue(((IMaybe)n).HasValue);
                Assert.AreEqual("123", ((IMaybe<string>)n).Value);
            }

            [Test]
            public async Task CanMapToNothing()
            {
                var m = Maybe.From(123);
                var n = await ((IMaybe<int>)m).MapAsync<string>(async x => Maybe.Nothing<string>());

                Assert.IsFalse(((IMaybe)n).HasValue);
                Assert.AreEqual(typeof(string), ((IMaybe)n).InternalType);
            }

            [Test]
            public async Task CanMapNothing()
            {
                var m = Maybe.Nothing<int>();
                var n = await ((IMaybe<int>)m).MapAsync<string>(async x => Maybe.From(x.ToString(CultureInfo.InvariantCulture)));

                Assert.IsFalse(((IMaybe)n).HasValue);
                Assert.AreEqual(typeof(string), ((IMaybe)n).InternalType);
            }

            [Test]
            public void RejectsNullBinder()
            {
                var m = Maybe.From(123);

                Assert.ThrowsAsync<ArgumentNullException>(async () => await ((IMaybe<int>)m).MapAsync<string>((Func<int, Task<Maybe<string>>>)null));
            }
        }
        
        [TestFixture]
        public class OrElse
        {
            [Test]
            public void CanGetValue()
            {
                var m = Maybe.From(123);
                Assert.AreEqual(123, m.OrElse(456));
            }

            [Test]
            public void RejectsInvalidArguments()
            {
                var m = Maybe.From(123);

                Assert.Throws<ArgumentNullException>(() => m.OrElse(null));
                Assert.ThrowsAsync<ArgumentNullException>(async () => await m.OrElseAsync(null));
            }

            [Test]
            public void CanGetAlternative()
            {
                var m = Maybe.Nothing<int>();
                Assert.AreEqual(456, m.OrElse(456));
            }

            [Test]
            public void CanGetValue_Factory()
            {
                var m = Maybe.From(123);
                Assert.AreEqual(123, m.OrElse(() =>
                {
                    Assert.Fail("Should never be reached.");
                    return 5;
                }));
            }

            [Test]
            public void CanGetAlternative_Factory()
            {
                var m = Maybe.Nothing<int>();
                Assert.AreEqual(456, m.OrElse(() => 456));
            }

            [Test]
            public async Task CanGetValue_Task()
            {
                var m = Maybe.From(123);
                Assert.AreEqual(123, await m.OrElseAsync(async () => 456));
            }

            [Test]
            public async Task CanGetAlternative_Task()
            {
                var m = Maybe.Nothing<int>();
                Assert.AreEqual(456, await m.OrElseAsync(async () => 456));
            }
        }

        [TestFixture]
        public class Equality
        {
            [Test]
            public void ProvidesHashCode()
            {
                var a = Maybe.From(123);
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

                var a = Maybe.From(o);
                Assert.AreEqual(-999, a.GetHashCode());

                o._hashCode = 777;
                Assert.AreEqual(777, a.GetHashCode());
            }

            [Test]
            public void ProvidesHashCode_Nothing()
            {
                var a = Maybe.Nothing<int>();
                Assert.AreEqual(typeof(int).GetHashCode(), a.GetHashCode());
            }

            [Test]
            public void Equality_MaybeOfT()
            {
                var a = Maybe.From(123);
                var b = Maybe.From(123);
                var c = Maybe.From(123);
                var d = Maybe.From(456);
                var e = Maybe.From("pretzels");
                var f = Maybe.Nothing<int>();
                var g = Maybe.Nothing<string>();

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
            public void Equality_IMaybeOfT()
            {
                IMaybe<int> a = Maybe.From(123);
                IMaybe<int> b = Maybe.From(123);
                IMaybe<int> c = Maybe.From(123);
                IMaybe<int> d = Maybe.From(456);
                IMaybe<string> e = Maybe.From("pretzels");
                IMaybe<int> f = Maybe.Nothing<int>();
                IMaybe<string> g = Maybe.Nothing<string>();

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
                IMaybe a = Maybe.From(123);
                IMaybe b = Maybe.From(123);
                IMaybe c = Maybe.From(123);
                IMaybe d = Maybe.From(456);
                IMaybe e = Maybe.From("pretzels");
                IMaybe f = Maybe.Nothing<int>();
                IMaybe g = Maybe.Nothing<string>();

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
                object a = Maybe.From(123);
                object b = Maybe.From(123);
                object c = Maybe.From(123);
                object d = Maybe.From(456);
                object e = Maybe.From("pretzels");
                object f = Maybe.Nothing<int>();
                object g = Maybe.Nothing<string>();

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
            public void Equality_Operators()
            {
                var a = Maybe.From(123);
                var b = Maybe.From(123);
                var c = Maybe.From(123);
                var d = Maybe.From(456);
                var e = Maybe.Nothing<int>();

                // reflexive
#pragma warning disable CS1718 // Comparison made to same variable
                Assert.IsTrue(a == a);
                Assert.IsFalse(a != a);
#pragma warning restore CS1718 // Comparison made to same variable

                // symmetric
                Assert.IsTrue(a == b);
                Assert.IsTrue(b == a);
                Assert.IsFalse(a != b);
                Assert.IsFalse(b != a);

                // reflexive
                Assert.IsTrue(a == b);
                Assert.IsTrue(b == c);
                Assert.IsTrue(a == c);
                Assert.IsFalse(a != b);
                Assert.IsFalse(b != c);
                Assert.IsFalse(a != c);

                // inequality
                Assert.IsFalse(a == d);
                Assert.IsFalse(a == e);
                Assert.IsTrue(a != d);
                Assert.IsTrue(a != e);
            }
        }

        [TestFixture]
        public class Infrastructure
        {
            public class MockParent { }
            
            public class MockChild : MockParent { }

            [Test]
            public void InterfaceHierarchy()
            {
                var m1 = Maybe.From(123);
                var m2 = Maybe.Nothing<int>();
                var m3 = (IMaybe<int>) m1;
                var m4 = (IMaybe<int>) m2;
                var m5 = Maybe.From(new MockChild());
                IMaybe<MockParent> m6 = m5;

                Assert.IsInstanceOf<IMaybe>(m1);
                Assert.IsInstanceOf<IMaybe>(m2);
                Assert.IsInstanceOf<IMaybe>(m3);
                Assert.IsInstanceOf<IMaybe>(m4);
                Assert.IsInstanceOf<IMaybe>(m5);
                Assert.IsInstanceOf<IMaybe>(m6);

                Assert.IsInstanceOf<IMaybe<int>>(m1);
                Assert.IsInstanceOf<IMaybe<int>>(m2);
                
                Assert.IsInstanceOf<IMaybe<object>>(m5);
                Assert.IsInstanceOf<IMaybe<object>>(m6);
            }

            [Test]
            public void ProvidesTypeInformation()
            {
                var m = Maybe.From(123);
                var n = Maybe.From("yada yada yada");

                var o = Maybe.Nothing<int>();
                var p = Maybe.Nothing<string>();

                Assert.AreEqual(typeof(int), ((IMaybe) m).InternalType);
                Assert.AreEqual(typeof(string), ((IMaybe) n).InternalType);

                Assert.AreEqual(typeof(int), ((IMaybe) o).InternalType);
                Assert.AreEqual(typeof(string), ((IMaybe) p).InternalType);
            }

            [Test]
            public void ProvidesValueInformation()
            {
                var m = Maybe.From(123);
                var n = Maybe.Nothing<int>();

                Assert.IsTrue(((IMaybe) m).HasValue);
                Assert.IsFalse(((IMaybe) n).HasValue);
            }
            
            [Test]
            public void CanGetValue_OfT_ViaProperty()
            {
                IMaybe<int> m = Maybe.From(123);
                IMaybe<string> n = Maybe.From("yada yada yada");

                var o = Maybe.Nothing<int>();
                var p = Maybe.Nothing<string>();

                Assert.AreEqual(123, m.Value);
                Assert.AreEqual("yada yada yada", n.Value);

                Assert.Throws<InvalidOperationException>(() => { var x = ((IMaybe<int>)o).Value; });
                Assert.Throws<InvalidOperationException>(() => { var x = ((IMaybe<string>)p).Value; });
            }

            [Test]
            public void CanGetValue_Object_ViaProperty()
            {
                IMaybe m = Maybe.From(123);
                IMaybe n = Maybe.From("yada yada yada");

                var o = Maybe.Nothing<int>();
                var p = Maybe.Nothing<string>();

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
                var m = Maybe.From("yada yada yada");
                var n = Maybe.Nothing<int>();

                Assert.AreEqual("yada yada yada", m.DebuggerDisplay);
                Assert.AreEqual("Nothing<Int32>", n.DebuggerDisplay);
            }

            [Test]
            public void ProvidesStringRepresentation()
            {
                var m = Maybe.From("yada yada yada");
                var n = Maybe.Nothing<int>();

                Assert.AreEqual("yada yada yada", m.ToString());
                Assert.AreEqual("Nothing<Int32>", n.ToString());
            }
        }
    }
}
