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
    /// <summary>
    /// Try monad of T.
    /// </summary>
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

        /// <summary>
        /// Adds the given <paramref name="binder"/> to this Try's call-sequence.
        /// </summary>
        public Try<U> Then<U>(Func<T, U> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new SyncTryFrame(TryFrameType.Action, x => binder((T) x), null);
            return new Try<U>(Frames.Add(frame));
        }

        /// <summary>
        /// Adds the given <paramref name="binder"/> to this Try's call-sequence.
        /// </summary>
        public Try<U> Then<U>(Func<U> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new SyncTryFrame(TryFrameType.Action, _ => binder(), null);
            return new Try<U>(Frames.Add(frame));
        }

        #endregion

        #region ThenAsync

        /// <summary>
        /// Adds the given <paramref name="binder"/> to this Try's call-sequence.
        /// </summary>
        public AsyncTry<U> ThenAsync<U>(Func<T, Task<U>> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var asyncFrames = Frames
                .ToAsync()
                .Add(new AsyncTryFrame(TryFrameType.Action, async x => await binder((T) x).ConfigureAwait(false), null));

            return new AsyncTry<U>(asyncFrames);
        }

        /// <summary>
        /// Adds the given <paramref name="binder"/> to this Try's call-sequence.
        /// </summary>
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

        /// <summary>
        /// Adds the given <paramref name="exceptionHandler"/> for exceptions assignable to type
        /// <typeparamref name="TException"/> to this Try's call sequence.
        /// </summary>
        public Try<T> Catch<TException>(Action<TException> exceptionHandler) where TException : Exception
        {
            TryThrowHelper.VerifyExceptionHandlerNotNull(exceptionHandler);

            var clause = new SyncCatchClause(
                ex => exceptionHandler((TException) ex),
                typeof(TException));

            return new Try<T>(Frames.Add(new SyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        /// <summary>
        /// Adds the given <paramref name="exceptionHandler"/> for exceptions assignable to type
        /// <paramref name="exceptionType"/> to this Try's call sequence.
        /// </summary>
        public Try<T> Catch(Type exceptionType, Action<Exception> exceptionHandler)
        {
            TryThrowHelper.VerifyExceptionType(exceptionType);
            TryThrowHelper.VerifyExceptionHandlerNotNull(exceptionHandler);

            var clause = new SyncCatchClause(
                ex => exceptionHandler(ex),
                exceptionType);

            return new Try<T>(Frames.Add(new SyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        #endregion

        #region CatchAsync

        /// <summary>
        /// Adds the given <paramref name="exceptionHandler"/> for exceptions assignable to type
        /// <typeparamref name="TException"/> to this Try's call sequence.
        /// </summary>
        public AsyncTry<T> CatchAsync<TException>(Func<TException, Task> exceptionHandler) where TException : Exception
        {
            TryThrowHelper.VerifyExceptionHandlerNotNull(exceptionHandler);

            var clause = new AsyncCatchClause(
                ex => exceptionHandler((TException) ex),
                typeof(TException));

            var asyncFrames = Frames
                .ToAsync()
                .Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause));

            return new AsyncTry<T>(asyncFrames);
        }

        /// <summary>
        /// Adds the given <paramref name="exceptionHandler"/> for exceptions assignable to type
        /// <paramref name="exceptionType"/> to this Try's call sequence.
        /// </summary>
        public AsyncTry<T> CatchAsync(Type exceptionType, Func<Exception, Task> exceptionHandler)
        {
            TryThrowHelper.VerifyExceptionType(exceptionType);
            TryThrowHelper.VerifyExceptionHandlerNotNull(exceptionHandler);

            var clause = new AsyncCatchClause(
                ex => exceptionHandler(ex),
                exceptionType);

            var asyncFrames = Frames
                .ToAsync()
                .Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause));

            return new AsyncTry<T>(asyncFrames);
        }

        #endregion

        #region Execute

        /// <summary>
        /// async/await infrastructure.
        /// Do not use directly.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ValueTaskAwaiter<Maybe<T>> GetAwaiter() => _ExecuteAsyncShim().GetAwaiter();

        private ValueTask<Maybe<T>> _ExecuteAsyncShim() => new ValueTask<Maybe<T>>(Execute());

        /// <summary>
        /// Executes all calls in this Try's call sequence and applies the exception
        /// handlers present in the call sequence. If the call sequence terminates normally,
        /// it returns a <see cref="Maybe{T}"/> that has a value, if it terminates with a
        /// caught exception it returns a Nothing, and if it terminates with an uncaught exception
        /// this exception is re-thrown.
        /// </summary>
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