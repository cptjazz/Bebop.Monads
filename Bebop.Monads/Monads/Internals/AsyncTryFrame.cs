// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.Threading.Tasks;

namespace Bebop.Monads.Internals
{
    internal sealed class AsyncTryFrame : TryFrame
    {
        public Func<object, Task<object>> Action { get; }

        public new AsyncCatchClause CatchClause => (AsyncCatchClause) base.CatchClause;

        public AsyncTryFrame(
            TryFrameType frameType,
            Func<object, Task<object>> action,
            AsyncCatchClause catchClause)
            : base(frameType, catchClause)
        {
            Action = action;
        }
    }
}