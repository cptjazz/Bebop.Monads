// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;

namespace Bebop.Monads.Internals
{
    internal sealed class AsyncCatchClause : CatchClause
    {
        public Func<Exception, Task<object>> Handler { get; }

        public AsyncCatchClause(Func<Exception, Task<object>> handler, Type exceptionType) 
            : base(exceptionType)
        {
            Handler = handler;
        }
    }
}
