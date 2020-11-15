// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bebop.Monads
{
    /// <summary>
    /// A factory for creating 'Try' instances.
    /// </summary>
    public static class Try
    {
        /// <summary>
        /// Creates a new Try and adds the given <paramref name="action"/> as first element
        /// to the Try's call sequence.
        /// </summary>
        public static Try<T> Do<T>(Func<T> action)
        {
            _ThrowIfNull(action);

            return new Try<T>(action);
        }

        /// <summary>
        /// Creates a new Try and adds the given <paramref name="action"/> as first element
        /// to the Try's call sequence.
        /// </summary>
        public static AsyncTry<T> DoAsync<T>(Func<Task<T>> action)
        {
            _ThrowIfNull(action);

            return new AsyncTry<T>(action);
        }

        [DebuggerStepThrough]
        private static void _ThrowIfNull(object action)
        {
            if (ReferenceEquals(action, null))
                throw new ArgumentNullException(nameof(action));
        }
    }
}
