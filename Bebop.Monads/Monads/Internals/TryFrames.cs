// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;

namespace Bebop.Monads.Internals
{
    internal abstract class TryFrames
    {
        protected TryFrame[] Frames { get; }

        public int Length => Frames.Length;

        protected TryFrames(TryFrame[] frames)
        {
            Frames = frames;
        }

        public int FindNextActionFrameIndex(int startIndex)
        {
            while (++startIndex < Frames.Length)
            {
                if (Frames[startIndex].FrameType == TryFrameType.Action)
                    return startIndex;
            }

            return Frames.Length;
        }

        public (CatchClause Clause, int Index) FindNextMatchingExceptionHandler(Exception exception, int startIndex)
        {
            while (++startIndex < Frames.Length)
            {
                if (Frames[startIndex].FrameType != TryFrameType.CatchClause)
                    continue;

                var clause = Frames[startIndex].CatchClause;

                if (clause.CanHandle(exception))
                    return (clause, startIndex);
            }

            return (null, int.MaxValue);
        }
    }
}
