# Bebop.Monads

This project provides the following monads:
* Maybe

This library is:
* netstandard2.0
* strong-name signed
* CLS compliant

[![NuGet](https://img.shields.io/badge/nuget-Bebop.Monads-blue.svg)](https://www.nuget.org/packages/Bebop.Monads) [![Build status](https://ci.appveyor.com/api/projects/status/5ygm0nc2uggl5adq/branch/master?svg=true)](https://ci.appveyor.com/project/cptjazz/bebop-monads/branch/master) [![Coverage Status](https://coveralls.io/repos/github/cptjazz/Bebop.Monads/badge.svg?branch=master)](https://coveralls.io/github/cptjazz/Bebop.Monads?branch=master)

## Maybe

This represents the _Maybe_ (or Option) monad.

### Construction

```C#
Maybe<string> m = Maybe.From("these pretzels are making me thirsty");
Maybe<int> n = Maybe.Nothing<int>(); 
```

### Binding
The method `Map` provides the monadic binding mechanism.

This example maps from an integer value to a string value:

```C#
Maybe<int> m = Maybe.From(123);
Maybe<string> n = m.Map(x => Maybe.From("something completely different")); // n represents the string
```

The next example does not execute the _binder_ (the lambda) because the source Maybe is empty. An empty target Maybe is created instead:

```C#
Maybe<int> m = Maybe.Nothing<int>();
Maybe<string> n = m.Map(x => Maybe.From("something completely different")); // n is Nothing
```

### Alternative Values
The Maybe allows for 'materialising' to a value of type T by providing an alternative value that is chosen if the Maybe does not have an internal value:

```C#
Maybe<int> m = Maybe.From(123);
int r = m.OrElse(456); // 123

Maybe<int> n = Maybe.Nothing<int>();
int s = n.OrElse(456); // 456
```

It is also possible to use lazy construction of the alternative value if creation of that value is costly:
```C#
Maybe<int> m = Maybe.From(123);
int r = m.OrElse(() => 456); // 123

Maybe<int> n = Maybe.Nothing<int>();
int s = n.OrElse(() => 456); // 456
```

### Async/Await Support

The binding (`Map(..)`) and alternative (`OrElse(..)`) methods are also available in async variants:

```C#
Maybe<int> m = Maybe.From(123);
Maybe<string> r1 = await m.MapAsync(async () => Maybe.From("A"));
Maybe<string> r2 = await m.MapAsync(() => new ValueTask<string>(Maybe.From("A")));
```

And 

```C#
Maybe<int> m = Maybe.From(123);
int r1 = await m.OrElseAsync(async () => 456); // 123
int r2 = await m.OrElseAsync(() => new ValueTask<int>(456)); // 123

Maybe<int> n = Maybe.Nothing<int>();
int s1 = await n.OrElseAsync(async () => 456); // 456
int s2 = await n.OrElseAsync(() => new ValueTask<int>(456)); // 456
```

### Infrastructure methods
To fit into the C#/.NET ecosystem we provide the following methods that go beyond pure monadic implementations. 
The methods are part of the interface `IMaybe<T>` and `IMaybe` and are implemented _explicitly_ i. e. you must cast to the interface to invoke the methods.

```C#
Maybe<string> m = Maybe.From("these pretzels are making me thirsty");
Maybe<int> n = Maybe.Nothing<int>(); 

((IMaybe) m).HasValue; // true
((IMaybe) n).HasValue; // false

((IMaybe) m).InternalType; // String
((IMaybe) n).InternalType; // Int32

((IMaybe) m).Value; // returns "these pretzels ..." -- return type is Object
((IMaybe) n).Value; // throws an InvalidOperationException -- cannot get the value of a Nothing

((IMaybe<string>) m).Value; // returns "these pretzels ..." -- return type is T
((IMaybe<int>) n).Value; // throws an InvalidOperationException -- cannot get the value of a Nothing

((IMaybe<string>) m).Map(x => ...)
((IMaybe<int>) n).Map(x => ...)
```

The interface `IMaybe<T>` is covariant:

```C#
private class A { }
private class B : A { }

IMaybe<A> o = Maybe.From(new B());
IMaybe<A> p = Maybe.Nothing<B>();
```

### Advanced Scenarios
When dealing with Maybes in scenarios where the types are not known at compile time we provide mechanisms to create 'non-generic' Maybes:

```C#
IMaybe m = Maybe.From(typeof(int), 123);
IMaybe n = Maybe.Nothing(typeof(string));
```
Those Maybes can only satisfy the basic `IMaybe` interface and thus do not offer the binding mechanism.

#### Down-castable non-generic Maybes
We can create instances of `IMaybe` during runtime, having only a `Type` and a value `object` in hand as described above. The returned instances satisfy exactly the `IMaybe` interface—nothing more. In fact, a different completely non-generic type is instantiated internally and returned by those methods. This is fast and satisfies exactly what the interface promises.

In some situations however it is beneficial to non-generically create instances that are true `Maybe<T>` instances that can be downcast from `IMaybe` to `Maybe<T>` during runtime. This library provides mechanisms to create true `Maybe<T>` objects by using Reflection internally. However, those methods are significantly slower than their not down-castable counterparts.

```C#
IMaybe m = Maybe.From(typeof(int), 123);
Maybe<int> n = (Maybe<int>) m; // fails!

// Using the Reflection based mechanisms:
IMaybe m = Maybe.Castable.From(typeof(int), 123);
Maybe<int> n = (Maybe<int>) m; // works!
```
