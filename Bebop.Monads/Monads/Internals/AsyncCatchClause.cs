// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;

namespace Bebop.Monads.Internals
{
    internal sealed class AsyncCatchClause : CatchClause
    {
        public Func<Exception, Task> Handler { get; }

        public AsyncCatchClause(Func<Exception, Task> handler, Type exceptionType) 
            : base(exceptionType)
        {
            Handler = handler;
        }
    }
}