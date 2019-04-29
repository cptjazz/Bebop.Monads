// Copyright 2018, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;

namespace Bebop.Monads
{
    /// <summary>
    /// Basic interface of the Maybe monad.
    /// This interface is usefull when accessing Maybes in a
    /// non-generic fashion, e. g. when processing lists of Maybes.
    /// </summary>
    public interface IMaybe
    {
        /// <summary>
        /// Returns 'True' if this <see cref="IMaybe"/> has a value,
        /// 'False' otherwise.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> argument the underlying <see cref="Maybe{T}"/> 
        /// was created with.
        /// </summary>
        Type InternalType { get; }
     
        /// <summary>
        /// Gets the internal value of this <see cref="IMaybe"/>,
        /// or the default value if this <see cref="IMaybe"/> is empty.
        /// </summary>
        [Obsolete("Use IMaybe.Value in combination with IMaybe.HasValue instead.")]
        object GetValueOrDefault();

        /// <summary>
        /// Gets the internal value of this <see cref="IMaybe"/>. 
        /// This property should only be queried after making sure that this
        /// <see cref="IMaybe"/> has a value by checking the <see cref="HasValue"/> 
        /// property!
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Throws an <see cref="InvalidOperationException"/> if this property is 
        /// called on a Nothing.
        /// </exception>
        object Value { get; }
    }
}