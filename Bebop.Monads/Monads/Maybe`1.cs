// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Diagnostics;

namespace Bebop.Monads
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct Maybe<T>
        : IEquatable<Maybe<T>>, IMaybe<T>, IMaybe
    {
        private readonly T _value;
        private readonly bool _hasValue;

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
        public T GetValueOrDefault()
        {
            return _hasValue ? _value : default;
        }

        IMaybe<U> IMaybe<T>.Map<U>(Func<T, Maybe<U>> binder)
        {
            return Map(binder);
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

        #endregion

        #region IMaybe interface

        bool IMaybe.HasValue => _hasValue;

        Type IMaybe.InternalType => typeof(T);

        object IMaybe.GetValueOrDefault()
        {
            return GetValueOrDefault();
        }

        #endregion

        #region Debugger

        internal string DebuggerDisplay => _hasValue 
            ? _value.ToString() 
            : $"Nothing<{typeof(T).Name}>";

        #endregion
    }
}