// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bebop.Monads
{
    /// <summary>
    /// Maybe monad of T.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly struct Maybe<T>
        : IEquatable<Maybe<T>>, IEquatable<IMaybe>, IMaybe<T>, IMaybe
    {
        private readonly bool _hasValue;
        private readonly T _value;

        #region Construction

        internal Maybe(in T value)
        {
            _value = value;
            _hasValue = true;
        }
        
        /// <summary>
        /// </summary>
        public static implicit operator Maybe<T>(T value)
        {
            if (ReferenceEquals(value, null))
                return default;

            return new Maybe<T>(value);
        }

        #endregion

        #region Equality

        /// <summary>Indicates whether the current object is equal to another
        /// object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the
        /// <paramref name="other">other</paramref> parameter;
        /// otherwise, false.</returns>
        public bool Equals(Maybe<T> other)
        {
            if (_hasValue)
                return other._hasValue && _value.Equals(other._value);

            return !other._hasValue;
        }

        /// <summary>Indicates whether the current object is equal to another
        /// object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the
        /// <paramref name="other">other</paramref> parameter;
        /// otherwise, false.</returns>
        public bool Equals(IMaybe other)
        {
            if (ReferenceEquals(other, null))
                return false;

            if (_hasValue)
                return other.HasValue && _value.Equals(other.Value);

            return !other.HasValue;
        }


        /// <summary>Indicates whether this instance and a specified object
        /// are equal.</summary>
        /// <param name="obj">The object to compare with the current
        /// instance.</param>
        /// <returns>true if <paramref name="obj">obj</paramref> and this
        /// instance are the same type and represent the same value;
        /// otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is IMaybe && Equals((IMaybe) obj);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for
        /// this instance.</returns>
        public override int GetHashCode()
        {
            return _hasValue 
                ? _value.GetHashCode()
                : typeof(T).GetHashCode();
        }

        /// <summary>
        /// </summary>
        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// </summary>
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

        /// <summary>
        /// Returns the internal value or <paramref name="alternative"/>
        /// if this <see cref="Maybe{T}"/> is Nothing.
        /// </summary>
        public T OrElse(
            T alternative)
        {
            return _hasValue
                ? _value
                : alternative;
        }

        /// <summary>
        /// Returns the internal value or constructs an alternative
        /// via the <paramref name="alternativeFactory"/> if this
        /// <see cref="Maybe{T}"/> is Nothing.
        /// </summary>
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

        object IMaybe.Value => _value;

        T IMaybe<T>.Value => _value;

        #endregion

        #region Debugger

        internal string DebuggerDisplay => _hasValue 
            ? _value.ToString() 
            : $"Nothing<{typeof(T).Name}>";

        #endregion

        #region String representation

        /// <summary>
        /// </summary>
        public override string ToString() => _hasValue
            ? _value.ToString()
            : $"Nothing<{typeof(T).Name}>";

        #endregion
    }
}