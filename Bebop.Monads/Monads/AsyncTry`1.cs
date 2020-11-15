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
    /// Asynchronous Try monad of T.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct AsyncTry<T>
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly AsyncTryFrames _frames;

        private AsyncTryFrames Frames => _frames ?? AsyncTryFrames.Empty;


        internal AsyncTry(Func<Task<T>> action)
        {
            var frame = new AsyncTryFrame(TryFrameType.Action, async _ => await action().ConfigureAwait(false), null);
            _frames = AsyncTryFrames.Empty.Add(frame);
        }

        internal AsyncTry(AsyncTryFrames frames)
        {
            _frames = frames;
        }

        #region Then

        /// <summary>
        /// Adds the given <paramref name="binder"/> to this Try's call sequence.
        /// </summary>
        public AsyncTry<U> Then<U>(Func<T, U> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new AsyncTryFrame(TryFrameType.Action, async x => binder((T)x), null);
            return new AsyncTry<U>(Frames.Add(frame));
        }

        /// <summary>
        /// Adds the given <paramref name="binder"/> to this Try's call sequence.
        /// </summary>
        public AsyncTry<U> Then<U>(Func<U> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new AsyncTryFrame(TryFrameType.Action, async x => binder(), null);
            return new AsyncTry<U>(Frames.Add(frame));
        }

        #endregion

        #region ThenAsync

        /// <summary>
        /// Adds the given <paramref name="binder"/> to this Try's call sequence.
        /// </summary>
        public AsyncTry<U> ThenAsync<U>(Func<T, Task<U>> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new AsyncTryFrame(TryFrameType.Action, async x => await binder((T)x).ConfigureAwait(false), null);
            return new AsyncTry<U>(Frames.Add(frame));
        }

        /// <summary>
        /// Adds the given <paramref name="binder"/> to this Try's call sequence.
        /// </summary>
        public AsyncTry<U> ThenAsync<U>(Func<Task<U>> binder)
        {
            TryThrowHelper.VerifyBinderNotNull(binder);

            var frame = new AsyncTryFrame(TryFrameType.Action, async x => await binder().ConfigureAwait(false), null);
            return new AsyncTry<U>(Frames.Add(frame));
        }

        #endregion

        #region Catch

        /// <summary>
        /// Adds the given <paramref name="exceptionHandler"/> for exceptions assignable to type
        /// <typeparamref name="TException"/> to this Try's call sequence.
        /// </summary>
        public AsyncTry<T> Catch<TException>(Func<TException, T> exceptionHandler) where TException : Exception
        {
            TryThrowHelper.VerifyExceptionHandlerNotNull(exceptionHandler);

            var clause = new AsyncCatchClause(
                async ex => exceptionHandler((TException)ex),
                typeof(TException));

            return new AsyncTry<T>(Frames.Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        /// <summary>
        /// Adds the given <paramref name="exceptionHandler"/> for exceptions assignable to type
        /// <paramref name="exceptionType"/> to this Try's call sequence.
        /// </summary>
        public AsyncTry<T> Catch(Type exceptionType, Func<Exception, T> exceptionHandler)
        {
            TryThrowHelper.VerifyExceptionType(exceptionType);
            TryThrowHelper.VerifyExceptionHandlerNotNull(exceptionHandler);

            var clause = new AsyncCatchClause(
                async ex => exceptionHandler(ex),
                exceptionType);

            return new AsyncTry<T>(Frames.Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        #endregion

        #region CatchAsync

        /// <summary>
        /// Adds the given <paramref name="exceptionHandler"/> for exceptions assignable to type
        /// <typeparamref name="TException"/> to this Try's call sequence.
        /// </summary>
        public AsyncTry<T> CatchAsync<TException>(Func<TException, Task<T>> exceptionHandler) where TException : Exception
        {
            TryThrowHelper.VerifyExceptionHandlerNotNull(exceptionHandler);

            var clause = new AsyncCatchClause(
                async ex => await exceptionHandler((TException)ex).ConfigureAwait(false),
                typeof(TException));

            return new AsyncTry<T>(Frames.Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        /// <summary>
        /// Adds the given <paramref name="exceptionHandler"/> for exceptions assignable to type
        /// <paramref name="exceptionType"/> to this Try's call sequence.
        /// </summary>
        public AsyncTry<T> CatchAsync(Type exceptionType, Func<Exception, Task<T>> exceptionHandler)
        {
            TryThrowHelper.VerifyExceptionType(exceptionType);
            TryThrowHelper.VerifyExceptionHandlerNotNull(exceptionHandler);

            var clause = new AsyncCatchClause(
                async ex => await exceptionHandler(ex).ConfigureAwait(false),
                exceptionType);

            return new AsyncTry<T>(Frames.Add(new AsyncTryFrame(TryFrameType.CatchClause, null, clause)));
        }

        #endregion

        #region Execute

        /// <summary>
        /// async/await infrastructure.
        /// Do not use directly.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TaskAwaiter<T> GetAwaiter() => ExecuteAsync().GetAwaiter();

        /// <summary>
        /// Executes all calls in this Try's call sequence and applies the exception
        /// handlers present in the call sequence. If the call sequence terminates normally,
        /// it returns a <see cref="Maybe{T}"/> that has a value, if it terminates with a
        /// caught exception it returns a Nothing, and if it terminates with an uncaught exception
        /// this exception is re-thrown.
        /// </summary>
        public async Task<T> ExecuteAsync()
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
                    var (c, i) = Frames.FindNextMatchingExceptionHandler(e, index);
                    var clause = (AsyncCatchClause)c;
                    index = i;

                    if (clause is null)
                        throw;

                    previousResult = await clause.Handler(e).ConfigureAwait(false);
                    index = Frames.FindNextActionFrameIndex(index);
                }
            }

            return previousResult is null
                ? default(T)
                : (T) previousResult;
        }

        #endregion
    }
}
