# Bebop.Monads

This project provides the following monads:
* Maybe
* Try

This library is:
* netstandard2.0
* strong-name signed
* CLS compliant

[![NuGet](https://img.shields.io/badge/nuget-Bebop.Monads-blue.svg)](https://www.nuget.org/packages/Bebop.Monads) [![Build status](https://ci.appveyor.com/api/projects/status/5ygm0nc2uggl5adq/branch/master?svg=true)](https://ci.appveyor.com/project/cptjazz/bebop-monads/branch/master) [![Coverage Status](https://coveralls.io/repos/github/cptjazz/Bebop.Monads/badge.svg?branch=master)](https://coveralls.io/github/cptjazz/Bebop.Monads?branch=master)

## Maybe

This represents the _Maybe_ (or Option) monad.

```C#
var m = Maybe.From("these pretzels are making me thirsty");
var n = Maybe.Nothing<int>(); 

var x = m.OrElse("yada yada yada"); // yields "these pretzels ..."
var y = n.OrElse(17); // yields 17

```
[See full documentation for `Maybe`](Maybe.md);

## Try

This represents an _exceptional_ monad that can be used to construct lazy chains of action and catch blocks.

```C#
var result = await Try
    .Do(() => "I will succeed.")
    .ThenAsync(async x => x + " Oh will you?")
    .Then(_ => throw new InvalidOperationException())
    .Catch<InvalidOperationException>(ex => 
    {
        Log.WriteError("Operation did not succeed");
        return "failed";
    });
```
[See full documentation for `Try`](Try.md);

## Unit

This represents the functional unit type.

```C#
var u = default(Unit);
```
[See full documentation for `Unit`](Unit.md);