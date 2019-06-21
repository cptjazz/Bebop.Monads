// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Bebop.Monads
{
    /// <summary>
    /// This interface provides covariance for <see cref="AsyncMaybe{T}"/>.
    /// Note: since <see cref="AsyncMaybe{T}"/> is a value type, calling methods
    /// via this interface introduces boxings!
    /// </summary>
    public interface IAsyncMaybe<T> : IAsyncMaybe
    {
        /// <summary>
        /// Applies the given async <paramref name="binder"/> to the internal value of this <see cref="IAsyncMaybe{T}"/>,
        /// or returns an empty <see cref="IAsyncMaybe{U}"/> (of the target type) if this <see cref="IAsyncMaybe{T}"/>
        /// is empty.
        /// </summary>
        IAsyncMaybe<U> MapAsync<U>(Func<T, Task<Maybe<U>>> binder);

        /// <summary>
        /// Applies the given <paramref name="binder"/> to the internal value of this <see cref="IAsyncMaybe{T}"/>,
        /// or returns an empty <see cref="IAsyncMaybe{U}"/> (of the target type) if this <see cref="IAsyncMaybe{T}"/>
        /// is empty.
        /// </summary>
        IAsyncMaybe<U> Map<U>(Func<T, Maybe<U>> binder);

        /// <summary>
        /// Provides a <see cref="ValueTask{T}"/> that represents the result of
        /// this <see cref="AsyncMaybe{T}"/>.
        /// </summary>
        Task<IMaybe<T>> AsTask();

        /// <summary>
        /// async/await infrastructure.
        /// Do not use directly.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        TaskAwaiter<IMaybe<T>> GetAwaiter();
    }
}