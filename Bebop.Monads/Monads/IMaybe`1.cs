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
        /// Gets the internal value of this <see cref="IMaybe{T}"/>. 
        /// This property should only be queried after making sure that this
        /// <see cref="IMaybe{T}"/> has a value by checking the <see cref="IMaybe.HasValue"/> 
        /// property!
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Throws an <see cref="InvalidOperationException"/> if this property is 
        /// called on a Nothing.
        /// </exception>
        new T Value { get; }

        /// <summary>
        /// Applies the given <paramref name="binder"/> to the internal value of this <see cref="IMaybe{T}"/>,
        /// or returns an empty <see cref="IMaybe{U}"/> (of the target type) if this <see cref="IMaybe{T}"/>
        /// is empty.
        /// </summary>
        IMaybe<U> Map<U>(Func<T, Maybe<U>> binder);

        /// <summary>
        /// Applies the given async <paramref name="binder"/> to the internal value of this <see cref="IMaybe{T}"/>
        /// and wraps the result in an <see cref="IAsyncMaybe{U}"/> that can be awaited, 
        /// or returns an empty <see cref="IAsyncMaybe{U}"/> (of the target type) if this <see cref="IMaybe{T}"/>
        /// is empty.
        /// </summary>
        IAsyncMaybe<U> MapAsync<U>(Func<T, Task<Maybe<U>>> binder);
    }
}