// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Diagnostics;

namespace Bebop.Monads.Internals
{
    [DebuggerStepThrough]
    internal static class MaybeThrowHelper
    {
        public static void VerifyBinderNotNull(object binder)
        {
            if (binder is null)
                _ThrowArgumentNull(nameof(binder));
        }

        public static void VerifyAlternativeFactoryNotNull(object alternativeFactory)
        {
            if (alternativeFactory is null)
                _ThrowArgumentNull(nameof(alternativeFactory));
        }

        private static void _ThrowArgumentNull(string parameterName)
        {
            throw new ArgumentNullException(parameterName);
        }
    }
}