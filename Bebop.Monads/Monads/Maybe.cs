// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Diagnostics;

namespace Bebop.Monads
{
    /// <summary>
    /// A factory for creating 'Maybe' instances.
    /// </summary>
    public static partial class Maybe
    {
        /// <summary>
        /// Creates an empty <see cref="Maybe{T}"/>.
        /// </summary>
        public static Maybe<T> Nothing<T>()
        {
            return default;
        }

        /// <summary>
        /// Creates an empty <see cref="IMaybe"/>.
        /// </summary>
        public static IMaybe Nothing(Type type)
        {
            _VerifyTypeNotNull(type);

            return new MaybeNongeneric(type);
        }

        /// <summary>
        /// Creates a <see cref="Maybe{T}"/> instance that contains the given <paramref name="value"/>.
        /// </summary>
        public static Maybe<T> From<T>(T value)
        {
            _VerifyNotNull(value);

            return new Maybe<T>(value);
        }

        /// <summary>
        /// Creates a <see cref="Maybe{T}"/> instance from the given nullable <paramref name="value"/>.
        /// </summary>
        public static Maybe<T> From<T>(Nullable<T> value) where T : struct
        {
            return value.HasValue 
                ? new Maybe<T>(value.Value) 
                : default;
        }

        /// <summary>
        /// Creates an <see cref="IMaybe"/> instance that contains the given <paramref name="value"/>.
        /// </summary>
        public static IMaybe From(Type type, object value)
        {
            _VerifyTypeNotNull(type);
            _VerifyNotNull(value);
            _VerifyValueAssignable(type, value);

            return new MaybeNongeneric(type, value);
        }

        [DebuggerStepThrough]
        private static void _VerifyValueAssignable(Type type, object value)
        {
            if (!type.IsAssignableFrom(value.GetType()))
                _ThrowNotAssignable(type, value);
        }

        [DebuggerStepThrough]
        private static void _ThrowNotAssignable(Type maybeType, object value)
        {
            var valueType = value.GetType();

            throw new ArgumentException(
                $"A Maybe of type '{maybeType}' cannot be constructed with " +
                $"a value of type '{valueType}'.");
        }

        [DebuggerStepThrough]
        private static void _VerifyNotNull<T>(T value)
        {
            if (ReferenceEquals(value, null))
                _ThrowValueNull(nameof(value));
        }

        [DebuggerStepThrough]
        private static void _ThrowValueNull(string parameterName)
        {
            throw new ArgumentNullException(
                parameterName,
                "Cannot construct a 'Maybe' from a 'null' value.");
        }

        [DebuggerStepThrough]
        private static void _VerifyTypeNotNull(Type type)
        {
            if (ReferenceEquals(type, null))
                _ThrowTypeNull(nameof(type));
        }

        [DebuggerStepThrough]
        private static void _ThrowTypeNull(string parameterName)
        {
            throw new ArgumentNullException(
                parameterName,
                "Type must not be null.");
        }
    }
}
