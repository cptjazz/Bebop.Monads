// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bebop.Monads
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly partial struct Maybe<T>
        : IEquatable<Maybe<T>>, IMaybe<T>, IMaybe
    {
        // internal for performance reasons
        private readonly bool _hasValue;
        private readonly T _value;

        #region Construction

        internal Maybe(in T value)
        {
            _value = value;
            _hasValue = true;
        }
        
        #endregion

        #region Equality

        public bool Equals(Maybe<T> other)
        {
            if (_hasValue)
                return _value.Equals(other._value);

            return !other._hasValue;
        }
        
        public override bool Equals(object obj)
        {
            return obj is Maybe<T> && Equals((Maybe<T>) obj);
        }

        public override int GetHashCode()
        {
            return _hasValue 
                ? _value.GetHashCode()
                : typeof(T).GetHashCode();
        }

        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Bind

        /// <summary>
        /// Gets the internal value of this <see cref="IMaybe{T}"/>,
        /// or the default value if this <see cref="IMaybe{T}"/> is empty.
        /// </summary>
        T IMaybe<T>.GetValueOrDefault()
        {
            return _hasValue ? _value : default;
        }

        IMaybe<U> IMaybe<T>.Map<U>(Func<T, Maybe<U>> binder)
        {
            return Map(binder);
        }

        async Task<IMaybe<U>> IMaybe<T>.Map<U>(Func<T, Task<Maybe<U>>> binder)
        {
            return await Map(binder);
        }

        /// <summary>
        /// Applies the given <paramref name="binder"/> to the internal value of this <see cref="Maybe{T}"/>,
        /// or returns an empty <see cref="Maybe{U}"/> (of the target type) if this <see cref="Maybe{T}"/>
        /// is empty.
        /// </summary>
        /// <param name="binder">A non-null binder.</param>
        public Maybe<U> Map<U>(in Func<T, Maybe<U>> binder)
        {
            if (binder is null)
                throw new ArgumentNullException(nameof(binder));

            return _hasValue ? binder(_value) : default;
        }

        /// <summary>
        /// Applies the given async <paramref name="binder"/> to the internal value of this <see cref="Maybe{T}"/>,
        /// or returns an empty <see cref="Maybe{U}"/> (of the target type) if this <see cref="Maybe{T}"/>
        /// is empty.
        /// </summary>
        /// <param name="binder">A non-null binder.</param>
        public Task<Maybe<U>> Map<U>(in Func<T, Task<Maybe<U>>> binder)
        {
            if (binder is null)
                throw new ArgumentNullException(nameof(binder));

            return _hasValue
                ? binder(_value)
                : Task.FromResult(default(Maybe<U>));
        }

        #endregion

        #region OrElse

        public T OrElse(
            T alternative)
        {
            return _hasValue
                ? _value
                : alternative;
        }

        public T OrElse(
            Func<T> alternativeFactory)
        {
            return _hasValue
                ? _value
                : alternativeFactory();
        }

        #endregion

        #region IMaybe interface

        bool IMaybe.HasValue => _hasValue;

        Type IMaybe.InternalType => typeof(T);

        object IMaybe.GetValueOrDefault()
        {
            return _hasValue ? _value : default;
        }

        #endregion

        #region Debugger

        internal string DebuggerDisplay => _hasValue 
            ? _value.ToString() 
            : $"Nothing<{typeof(T).Name}>";

        #endregion
    }
}