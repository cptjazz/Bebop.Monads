// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Diagnostics;

namespace Bebop.Monads.Internals
{
    [DebuggerStepThrough]
    internal static class TryThrowHelper
    {
        public static void VerifyExceptionType(Type exceptionType)
        {
            if (exceptionType is null) 
                _ThrowArgumentNull(nameof(exceptionType));

            if (!typeof(Exception).IsAssignableFrom(exceptionType))
            {
                throw new ArgumentException(
                    $"The given type must be assignable to '{typeof(Exception).FullName}'.",
                    nameof(exceptionType));
            }
        }

        public static void VerifyExceptionHandlerNotNull(object handler)
        {
            if (handler is null) 
                _ThrowArgumentNull(nameof(handler));
        }

        public static void VerifyBinderNotNull(object binder)
        {
            if (binder is null)
                _ThrowArgumentNull(nameof(binder));
        }

        private static void _ThrowArgumentNull(string parameterName)
        {
            throw new ArgumentNullException(parameterName);
        }
    }
}