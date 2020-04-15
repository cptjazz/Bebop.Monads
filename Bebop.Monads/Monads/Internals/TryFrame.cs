// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

namespace Bebop.Monads.Internals
{
    internal abstract class TryFrame
    {
        public TryFrameType FrameType { get; }
        
        public CatchClause CatchClause { get; }

        public TryFrame(
            TryFrameType frameType,
            CatchClause catchClause)
        {
            FrameType = frameType;
            CatchClause = catchClause;
        }
    }
}