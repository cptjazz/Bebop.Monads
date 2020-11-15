// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;

namespace Bebop.Monads.Internals
{
    internal sealed class AsyncTryFrames : TryFrames
    {
        public static readonly AsyncTryFrames Empty = new AsyncTryFrames(Array.Empty<AsyncTryFrame>());

        internal AsyncTryFrames(AsyncTryFrame[] frames)
            : base(frames)
        {
        }

        public AsyncTryFrame this[int index] => (AsyncTryFrame) Frames[index];

        public AsyncTryFrames Add(AsyncTryFrame frame)
        {
            var clausesLength = Frames.Length;
            
            var arr = new AsyncTryFrame[clausesLength + 1];
            Array.Copy(Frames, arr, clausesLength);
            arr[clausesLength] = frame;

            return new AsyncTryFrames(arr);
        }
    }
}