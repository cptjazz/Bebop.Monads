// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;

namespace Bebop.Monads
{
    /// <summary>
    /// Basic interface of the AsyncMaybe monad.
    /// This interface is usefull when accessing AsyncMaybes in a
    /// non-generic fashion.
    /// </summary>
    public interface IAsyncMaybe
    {
        /// <summary>
        /// Gets the <see cref="Type"/> argument the underlying <see cref="AsyncMaybe{T}"/> 
        /// was created with.
        /// </summary>
        Type InternalType { get; }
    }
}