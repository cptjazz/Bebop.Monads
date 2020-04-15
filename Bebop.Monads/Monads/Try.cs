// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Bebop.Monads
{
    public static class Try
    {
        public static Try<T> To<T>(Func<T> action)
        {
            _ThrowIfNull(action);

            return new Try<T>(action);
        }

        public static AsyncTry<T> ToAsync<T>(Func<Task<T>> action)
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
