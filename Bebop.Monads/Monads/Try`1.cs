// Copyright 2020, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Bebop.Monads.Internals;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Bebop.Monads
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Try<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly SyncTryFrames _frames;

        private SyncTryFrames Frames => _frames ?? SyncTryFrames.Empty;


        internal Try(Func<T> action)
        {
            var frame = new SyncTryFrame(TryFrameType.Action, _ => action(), null);
            _frames = SyncTryFrames.Empty.Add(frame);
        }

        internal Try(SyncTryFrames frames)
        {
            _frames = frames;
        }

        #region Then

        public Try<U> Then<U>(Func<T, U> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new SyncTryFrame(TryFrameType.Action, x => binder((T) x), null);
            return new Try<U>(Frames.Add(frame));
        }

        public Try<U> Then<U>(Func<U> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new SyncTryFrame(TryFrameType.Action, _ => binder(), null);
            return new Try<U>(Frames.Add(frame));
        }
        
        #endregion

        #region ThenAsync

        public AsyncTry<U> ThenAsync<U>(Func<T, Task<U>> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var asyncFrames = Frames
                .ToAsync()
                .Add(new AsyncTryFrame(TryFrameType.Action, async x => await binder((T) x).ConfigureAwait(false), null));

            return new AsyncTry<U>(asyncFrames);
        }

        public AsyncTry<U> ThenAsync<U>(Func<Task<U>> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var asyncFrames = Frames
                .ToAsync()
                .Add(new AsyncTryFrame(TryFrameType.Action, async x => await binder().ConfigureAwait(false), null));

            return new AsyncTry<U>(asyncFrames);
        }

        #endregion

        #region Catch

        public Try<T> Catch<TException>(Action<TException> handler) where TException : Exception
        {
            TryThrowHelper.VerifyExceptionHandlerNotNull(handler);

            var clause = new SyncCatchClause(
                ex => handler((TException) ex),
                typeof(TException));

            return new Try<T>(Frames.Add(new SyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        public Try<T> Catch(Type exceptionType, Action<Exception> handler)
        {
            TryThrowHelper.VerifyExceptionType(exceptionType);
            TryThrowHelper.VerifyExceptionHandlerNotNull(handler);

            var clause = new SyncCatchClause(
                ex => handler(ex),
                exceptionType);

            return new Try<T>(Frames.Add(new SyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        #endregion

        #region CatchAsync

        public AsyncTry<T> CatchAsync<TException>(Func<TException, Task> handler) where TException : Exception
        {
            TryThrowHelper.VerifyExceptionHandlerNotNull(handler);

            var clause = new AsyncCatchClause(
                ex => handler((TException) ex),
                typeof(TException));

            var asyncFrames = Frames
                .ToAsync()
                .Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause));

            return new AsyncTry<T>(asyncFrames);
        }

        public AsyncTry<T> CatchAsync(Type exceptionType, Func<Exception, Task> handler)
        {
            TryThrowHelper.VerifyExceptionType(exceptionType);
            TryThrowHelper.VerifyExceptionHandlerNotNull(handler);

            var clause = new AsyncCatchClause(
                ex => handler(ex),
                exceptionType);

            var asyncFrames = Frames
                .ToAsync()
                .Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause));

            return new AsyncTry<T>(asyncFrames);
        }

        #endregion

        #region Execute

        [EditorBrowsable(EditorBrowsableState.Never)]
        public TaskAwaiter<Maybe<T>> GetAwaiter() => _ExecuteAsyncShim().GetAwaiter();

        private async Task<Maybe<T>> _ExecuteAsyncShim() => Execute();

        public Maybe<T> Execute()
        {
            object previousResult = null;
            var index = Frames.FindNextActionFrameIndex(-1);

            while (index < Frames.Length)
            {
                var frame = Frames[index];

                try
                {
                    previousResult = frame.Action(previousResult);
                    index = Frames.FindNextActionFrameIndex(index);
                }
                catch (Exception e)
                {
                    var clause = (SyncCatchClause) Frames.FindNextMatchingExceptionHandler(e, index);
                    if (clause is null)
                        throw;

                    clause.Handler(e);
                    return Maybe.Nothing<T>();
                }
            }

            return previousResult is null
                ? Maybe.Nothing<T>()
                : Maybe.From((T) previousResult);
        }

        #endregion
    }
}