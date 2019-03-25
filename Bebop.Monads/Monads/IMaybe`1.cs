// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;

namespace Bebop.Monads
{
    /// <summary>
    /// This interface provides covariance for <see cref="Maybe{T}"/>.
    /// Note: since <see cref="Maybe{T}"/> is a value type, calling methods
    /// via this interface introduces boxings!
    /// </summary>
    public interface IMaybe<out T> : IMaybe
    {
        /// <summary>
        /// Gets the internal value of this <see cref="IMaybe{T}"/>,
        /// or the default value if this <see cref="IMaybe{T}"/> is empty.
        /// </summary>
        [Obsolete("Use IMaybe`1.Value in combination with IMaybe`1.HasValue instead.")]
        new T GetValueOrDefault();

        /// <summary>
        /// Gets the internal value of this <see cref="IMaybe{T}"/>. 
        /// This property should only be queried after making sure that this
        /// <see cref="IMaybe{T}"/> has a value by checking the <see cref="IMaybe.HasValue"/> 
        /// property! The exact return value for an empty <see cref="IMaybe{T}"/> 
        /// depends on the internal implementation and remains hereby unspecified.
        /// </summary>
        new T Value { get; }

        /// <summary>
        /// Applies the given <paramref name="binder"/> to the internal value of this <see cref="IMaybe{T}"/>,
        /// or returns an empty <see cref="IMaybe{U}"/> (of the target type) if this <see cref="IMaybe{T}"/>
        /// is empty.
        /// </summary>
        /// <param name="binder">A non-null binder.</param>
        IMaybe<U> Map<U>(Func<T, Maybe<U>> binder);

        /// <summary>
        /// Applies the given async <paramref name="binder"/> to the internal value of this <see cref="IMaybe{T}"/>,
        /// or returns an empty <see cref="IMaybe{U}"/> (of the target type) if this <see cref="IMaybe{T}"/>
        /// is empty.
        /// </summary>
        /// <param name="binder">A non-null binder.</param>
        Task<IMaybe<U>> Map<U>(Func<T, Task<Maybe<U>>> binder);
    }
}