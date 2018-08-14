# Bebop.Monads

This project provides the following monad implementations:
* Maybe

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

This example does not execute the _binder_ (the lambda) because the source Maybe is empty. An empty target Maybe is created instead:

```C#
Maybe<int> m = Maybe.Nothing<int>();
Maybe<string> n = m.Map(x => Maybe.From("something completely different")); // n is Nothing
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

((IMaybe) m).GetValueOrDefault(); // returns "these pretzels ..." -- return type is Object
((IMaybe) n).GetValueOrDefault(); // returns default(int) -- return type is Object

((IMaybe<string>) m).GetValueOrDefault(); // returns "these pretzels ..." -- return type is T
((IMaybe<int>) n).GetValueOrDefault(); // returns default(int) -- return type is T

((IMaybe<string>) m).Map(x => ...)
((IMaybe<int>) n).Map(x => ...)
```

The interface `IMaybe<T>` is covariant:

```
private class A { }
private class B : A { }

IMaybe<A> o = Maybe.From(new B());
IMaybe<A> p = Maybe.Nothing<B>();
```

### Advanced Scenarios
When dealing with Maybes in scenarios where the types are not known at compile time we provide mechanisms to create 'non-generic' Maybes:

```C#
IMaybe m = Maybe.From(typeof(int), "123");
IMaybe n = Maybe.Nothing(typeof(string));
```
Those Maybes can only satisfy the basic `IMaybe` interface and thus do not offer the binding mechanism.
