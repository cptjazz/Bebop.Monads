## Try

This represents the _Try_ (or Exceptional) monad.


### Construction and Binding

The _Try_ monad allows for defining a chain of lazily executed operations that are executed sequentially.
In case of exceptions, the next appropriate (i. e. assignment compatible) _catch_ handler will be invoked.
The whole `Try`construction and execution is primarily regarded an expression, this means every catch-handler
_must_ return a value.

```C#
Try<int> tryResult = Try
    .Do(_SomeMethod)
    .Then(_SomeOtherMethodThatThrows)
    .Then(_AndAnotherMethod) // assuming _AndAnotherMethod() returns an int
    .Catch<InvalidOperationException>(ex => 
    {
        Log.WriteError("Operation did not succeed");
        return -99;
    });
```

The constructed `tryResult` will not be executed until `Execute()` is invoked on it.

```C#
int result = tryResult.Execute();
```

The result of the `Execute()` operation is a ``Maybe`1`` because generation of a value is not
guaranteed when an exception occurs.

If no apropriate catch-handler is found, execution of the `Try` throws:

```C#
var result = Try
    .Do<int>(() => throw new ArithmeticException())
    .Catch<InvalidOperationException>(ex => 
    {
        Log.WriteError("Operation did not succeed");
        return -99;
    })
    .Execute(); // throws an ArithmeticException
```

Execution of an empty ``Try`1`` leads to `default(T)`:

```C#
var result = default(Try<string>).Execute(); 
// result is default(string)
```

### Async/Await Support

There is an ``async`` variant for all construction and binding methods.

```C#
var result = await Try
    .DoAsync(_SomeAsyncMethod)
    .ThenAsync(_SomeOtherAsyncMethodThatThrows)
    .ThenAsync(_AndAnotherAsyncMethod)
    .CatchAsync<InvalidOperationException>(async ex => 
    {
        Log.WriteError("Operation did not succeed");
        return -99;
    })
    .ExecuteAsync();
```

The explicit calls to `.Execute()` and `.ExecuteAsync()` can be 'replaced' by an `await` because 
both ``Try`1`` and ``AsyncTry`1`` are awaitables:

```C#
var result = await Try
    .DoAsync(_SomeAsyncMethod)
    .ThenAsync(_SomeOtherAsyncMethodThatThrows)
    .ThenAsync(_AndAnotherAsyncMethod)
    .CatchAsync<InvalidOperationException>(async ex =>
    {
        Log.WriteError("Operation did not succeed");
        return -99;
    });
```

### Infrastructure Methods

All ``.Catch<T>(..)`` and ``.CatchAsync<T>(..)`` variants also provide overloads that allow for specifying
the exception type as `Type` object.

```C#
var result = await Try
    .DoAsync(_SomeAsyncMethod)
    .CatchAsync(
        typeof(InvalidOperationException),
        async ex =>
        {
            Log.WriteError("Operation did not succeed");
            return -99;
        });
```