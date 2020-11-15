// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;

#pragma warning disable CS1998

namespace Bebop.Monads.Internals
{
    internal sealed class SyncCatchClause : CatchClause
    {
        public Func<Exception, object> Handler { get; }


        public SyncCatchClause(Func<Exception, object> handler, Type exceptionType) 
            : base(exceptionType)
        {
            Handler = handler;
        }

        public AsyncCatchClause ToAsync()
        {
            return new AsyncCatchClause(
                async ex => Handler(ex),
                ExceptionType);
        }
    }
}
