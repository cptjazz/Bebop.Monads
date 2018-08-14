using System;
using System.Reflection;

namespace Bebop.Monads
{
    /// <summary>
    /// A factory for creating 'Maybe' instances.
    /// </summary>
    public static class Maybe
    {
        /// <summary>
        /// Creates an empty <see cref="Maybe{T}"/>.
        /// </summary>
        public static Maybe<T> Nothing<T>()
        {
            return default;
        }

        /// <summary>
        /// Creates an empty <see cref="Maybe{T}"/>.
        /// </summary>
        public static IMaybe Nothing(Type type)
        {
            _VerifyTypeNotNull(type);

            return (IMaybe) Activator.CreateInstance(typeof(Maybe<>).MakeGenericType(type));
        }

        /// <summary>
        /// Creates a <see cref="Maybe{T}"/> instance that contains the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">A non-null value.</param>
        public static Maybe<T> From<T>(in T value)
        {
            _VerifyNotNull(value);

            return new Maybe<T>(value);
        }

        /// <summary>
        /// Creates an empty <see cref="Maybe{T}"/>.
        /// </summary>
        public static IMaybe From(Type type, object value)
        {
            _VerifyTypeNotNull(type);
            _VerifyNotNull(value);
            _VerifyValueAssignable(type, value);

            return (IMaybe)Activator.CreateInstance(
                typeof(Maybe<>).MakeGenericType(type),
                new object[] { value });
        }

        private static void _VerifyValueAssignable(Type type, object value)
        {
            if (!type.GetTypeInfo().IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException(
                   $"A Maybe of type '{type}' cannot be constructed with " +
                   $"a value of type '{value.GetType()}'.");
            }
        }

        private static void _VerifyNotNull<T>(in T value)
        {
            if (ReferenceEquals(value, null))
            {
                throw new ArgumentNullException(
                    nameof(value),
                    "Cannot construct a 'Maybe' from a 'null' value.");
            }
        }

        private static void _VerifyTypeNotNull(in Type type)
        {
            if (ReferenceEquals(type, null))
            {
                throw new ArgumentNullException(
                    nameof(type),
                    "Type must not be null.");
            }
        }
    }
}
