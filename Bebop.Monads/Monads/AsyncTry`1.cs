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
    public readonly struct AsyncTry<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly AsyncTryFrames _frames;

        private AsyncTryFrames Frames => _frames ?? AsyncTryFrames.Empty;


        public AsyncTry(Func<Task<T>> action)
        {
            var frame = new AsyncTryFrame(TryFrameType.Action, async _ => await action().ConfigureAwait(false), null);
            _frames = AsyncTryFrames.Empty.Add(frame);
        }

        internal AsyncTry(AsyncTryFrames frames)
        {
            _frames = frames;
        }

        #region Then

        public AsyncTry<U> Then<U>(Func<T, U> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new AsyncTryFrame(TryFrameType.Action, async x => binder((T)x), null);
            return new AsyncTry<U>(Frames.Add(frame));
        }

        public AsyncTry<U> Then<U>(Func<U> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new AsyncTryFrame(TryFrameType.Action, async x => binder(), null);
            return new AsyncTry<U>(Frames.Add(frame));
        }

        #endregion

        #region ThenAsync

        public AsyncTry<U> ThenAsync<U>(Func<T, Task<U>> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new AsyncTryFrame(TryFrameType.Action, async x => await binder((T)x).ConfigureAwait(false), null);
            return new AsyncTry<U>(Frames.Add(frame));
        }

        public AsyncTry<U> ThenAsync<U>(Func<Task<U>> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new AsyncTryFrame(TryFrameType.Action, async x => await binder().ConfigureAwait(false), null);
            return new AsyncTry<U>(Frames.Add(frame));
        }

        #endregion

        #region Catch

        public AsyncTry<T> Catch<TException>(Action<TException> handler) where TException : Exception
        {
            TryThrowHelper.VerifyExceptionHandlerNotNull(handler);

            var clause = new AsyncCatchClause(
                async ex => handler((TException)ex),
                typeof(TException));

            return new AsyncTry<T>(Frames.Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        public AsyncTry<T> Catch(Type exceptionType, Action<Exception> handler)
        {
            TryThrowHelper.VerifyExceptionType(exceptionType);
            TryThrowHelper.VerifyExceptionHandlerNotNull(handler);

            var clause = new AsyncCatchClause(
                async ex => handler(ex),
                exceptionType);

            return new AsyncTry<T>(Frames.Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        #endregion

        #region CatchAsync

        public AsyncTry<T> CatchAsync<TException>(Func<TException, Task> handler) where TException : Exception
        {
            TryThrowHelper.VerifyExceptionHandlerNotNull(handler);

            var clause = new AsyncCatchClause(
                async ex => await handler((TException)ex).ConfigureAwait(false),
                typeof(TException));

            return new AsyncTry<T>(Frames.Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        public AsyncTry<T> CatchAsync(Type exceptionType, Func<Exception, Task> handler)
        {
            TryThrowHelper.VerifyExceptionType(exceptionType);
            TryThrowHelper.VerifyExceptionHandlerNotNull(handler);

            var clause = new AsyncCatchClause(
                async ex => await handler(ex).ConfigureAwait(false),
                exceptionType);

            return new AsyncTry<T>(Frames.Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        #endregion

        #region Execute

        [EditorBrowsable(EditorBrowsableState.Never)]
        public TaskAwaiter<Maybe<T>> GetAwaiter() => ExecuteAsync().GetAwaiter();

        public async Task<Maybe<T>> ExecuteAsync()
        {
            object previousResult = null;
            var index = Frames.FindNextActionFrameIndex(-1);

            while (index < Frames.Length)
            {
                var frame = Frames[index];

                try
                {
                    previousResult = await frame.Action(previousResult).ConfigureAwait(false);
                    index = Frames.FindNextActionFrameIndex(index);
                }
                catch (Exception e)
                {
                    var clause = (AsyncCatchClause) Frames.FindNextMatchingExceptionHandler(e, index);
                    if (clause is null)
                        throw;

                    await clause.Handler(e).ConfigureAwait(false);
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