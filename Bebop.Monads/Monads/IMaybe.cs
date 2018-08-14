using System;

namespace Bebop.Monads
{
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
        object GetValueOrDefault();
    }
}