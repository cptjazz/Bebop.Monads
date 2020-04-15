// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;

namespace Bebop.Monads.Internals
{
    internal sealed class SyncTryFrame : TryFrame
    {
        public Func<object, object> Action { get; }

        public new SyncCatchClause CatchClause => (SyncCatchClause) base.CatchClause;


        public SyncTryFrame(
            TryFrameType frameType,
            Func<object, object> action,
            SyncCatchClause catchClause)
            : base(frameType, catchClause)
        {
            Action = action;
        }
    }
}