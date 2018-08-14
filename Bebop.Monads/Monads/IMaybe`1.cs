using System;

namespace Bebop.Monads
{
    public interface IMaybe<out T>
    {
        /// <summary>
        /// Gets the internal value of this <see cref="IMaybe{T}"/>,
        /// or the default value if this <see cref="IMaybe{T}"/> is empty.
        /// </summary>
        T GetValueOrDefault();

        /// <summary>
        /// Applies the given <paramref name="binder"/> to the internal value of this <see cref="IMaybe{T}"/>,
        /// or returns an empty <see cref="IMaybe{U}"/> (of the target type) if this <see cref="IMaybe{T}"/>
        /// is empty.
        /// </summary>
        /// <param name="binder">A non-null binder.</param>
        IMaybe<U> Map<U>(Func<T, Maybe<U>> binder);
    }
}