## Unit

This represents the functional [unit type](https://en.wikipedia.org/wiki/Unit_type).

### Construction

```C#
Unit u = default;
```

### Advanced Scenarios
`Unit` is implemented as a _value type_. In some situations this might lead to unwanted boxing operations.
This library provides a "pre boxed" shared instance of the unit type that can be used in such scenarios.

```C#
IUnit u = Unit.Instance;
```
