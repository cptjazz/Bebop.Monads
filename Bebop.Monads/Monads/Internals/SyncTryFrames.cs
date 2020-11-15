// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;

#pragma warning disable CS1998

namespace Bebop.Monads.Internals
{
    internal sealed class SyncTryFrames : TryFrames
    {
        public static readonly SyncTryFrames Empty = new SyncTryFrames(Array.Empty<SyncTryFrame>());
        
        public SyncTryFrames(SyncTryFrame[] frames) 
            : base(frames)
        {
        }

        public SyncTryFrame this[int index] => (SyncTryFrame) Frames[index];

        public SyncTryFrames Add(SyncTryFrame frame)
        {
            var clausesLength = Frames.Length;
            
            var arr = new SyncTryFrame[clausesLength + 1];
            Array.Copy(Frames, arr, clausesLength);
            arr[clausesLength] = frame;

            return new SyncTryFrames(arr);
        }

        public AsyncTryFrames ToAsync()
        {
            var framesLength = Frames.Length;
            var frames = new AsyncTryFrame[framesLength];

            for (int i = 0; i < framesLength; i++)
            {
                var cur = (SyncTryFrame) Frames[i];
                
                if (cur.FrameType == TryFrameType.Action)
                    frames[i] = new AsyncTryFrame(cur.FrameType, async x => cur.Action(x), null);
                else
                {
                    frames[i] = new AsyncTryFrame(TryFrameType.CatchClause, null, cur.CatchClause.ToAsync());
                }
            }

            return new AsyncTryFrames(frames);
        }
    }
}