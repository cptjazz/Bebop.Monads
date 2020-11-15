// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;

namespace Bebop.Monads.Internals
{
    internal abstract class CatchClause
    {
        public Type ExceptionType { get; }

        public CatchClause(Type exceptionType)
        {
            ExceptionType = exceptionType;
        }

        public bool CanHandle(Exception exception)
        {
            return ExceptionType.IsAssignableFrom(exception.GetType());
        }
    }
}