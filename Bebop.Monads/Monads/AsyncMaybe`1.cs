﻿// Copyright 2019, Alexander Jesner
// License: https://opensource.org/licenses/MIT

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Bebop.Monads.Internals;

namespace Bebop.Monads
{
    /// <summary>
    /// Asynchronous Maybe monad of T.
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public readonly struct AsyncMaybe<T> : IAsyncMaybe<T>
    {
        private readonly Task<Maybe<T>> _task;

        #region Construction

        internal AsyncMaybe(Task<Maybe<T>> task)
        {
            _task = task;
        }
                
        #endregion

        #region IAsyncMaybe interface

        Type IAsyncMaybe.InternalType => typeof(T);

        #endregion

        #region IAsyncMaybe`1 interface

        IAsyncMaybe<U> IAsyncMaybe<T>.Map<U>(Func<T, Maybe<U>> binder)
        {
            return Map(binder);
        }

        IAsyncMaybe<U> IAsyncMaybe<T>.MapAsync<U>(Func<T, Task<Maybe<U>>> binder)
        {
            return MapAsync(binder);
        }

        #endregion

        #region Bind
        
        /// <summary>
        /// Applies the given <paramref name="binder"/> to the internal value of this <see cref="AsyncMaybe{T}"/>,
        /// and wraps the result in an <see cref="IAsyncMaybe{U}"/> that can be awaited, 
        /// or returns an empty <see cref="AsyncMaybe{U}"/> (of the target type) if this <see cref="AsyncMaybe{T}"/>
        /// is empty.
        /// </summary>
        public AsyncMaybe<U> MapAsync<U>(Func<T, Task<Maybe<U>>> binder)
        {
            MaybeThrowHelper.VerifyBinderNotNull(binder);

            if (_task is null)
                return default;

            return new AsyncMaybe<U>(_ProduceValueAsync(binder));
        }

        /// <summary>
        /// Applies the given async <paramref name="binder"/> to the internal value of this <see cref="AsyncMaybe{T}"/>,
        /// and wraps the result in an <see cref="IAsyncMaybe{U}"/> that can be awaited, 
        /// or returns an empty <see cref="AsyncMaybe{U}"/> (of the target type) if this <see cref="AsyncMaybe{T}"/>
        /// is empty.
        /// </summary>
        public AsyncMaybe<U> Map<U>(Func<T, Maybe<U>> binder)
        {
            MaybeThrowHelper.VerifyBinderNotNull(binder);

            if (_task is null)
                return default;

            return new AsyncMaybe<U>(_ProduceValueSync(binder));
        }

        private async Task<Maybe<U>> _ProduceValueAsync<U>(Func<T, Task<Maybe<U>>> binder)
        {
            await Task.Yield();

            var value = await _task.ConfigureAwait(false);
            return await value
                .MapAsync(binder)
                .AsValueTask()
                .ConfigureAwait(false);
        }

        private async Task<Maybe<U>> _ProduceValueSync<U>(Func<T, Maybe<U>> binder)
        {
            await Task.Yield();

            var value = await _task.ConfigureAwait(false);
            return value.Map(binder);
        }

        #endregion

        #region Task

        async Task<IMaybe<T>> IAsyncMaybe<T>.AsTask()
        {
            if (_task is null)
                return Maybe.Nothing<T>();

            return await _task.ConfigureAwait(false);
        }

        /// <summary>
        /// Provides a <see cref="ValueTask{T}"/> that represents the result of
        /// this <see cref="AsyncMaybe{T}"/>.
        /// </summary>
        public ValueTask<Maybe<T>> AsValueTask()
        {
            if (_task is null)
                return new ValueTask<Maybe<T>>(Maybe.Nothing<T>());

            return new ValueTask<Maybe<T>>(_task);
        }

        /// <summary>
        /// async/await infrastructure.
        /// Do not use directly.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public ValueTaskAwaiter<Maybe<T>> GetAwaiter()
        {
            return AsValueTask().GetAwaiter();
        }

        /// <summary>
        /// async/await infrastructure.
        /// Do not use directly.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        TaskAwaiter<IMaybe<T>> IAsyncMaybe<T>.GetAwaiter()
        {
            return ((IAsyncMaybe<T>) this).AsTask().GetAwaiter();
        }

        #endregion

        #region OrElse

        /// <summary>
        /// Returns the internal value or constructs an alternative
        /// via the <paramref name="alternative"/> if this
        /// <see cref="AsyncMaybe{T}"/> is Nothing.
        /// </summary>
        public ValueTask<T> OrElse(T alternative)
        {
            if (_task is null)
                return new ValueTask<T>(alternative);

            return new ValueTask<T>(_AwaitOrElse(alternative));
        }

        /// <summary>
        /// Returns the internal value or constructs an alternative
        /// via the <paramref name="alternativeFactory"/> if this
        /// <see cref="AsyncMaybe{T}"/> is Nothing.
        /// </summary>
        public ValueTask<T> OrElse(
            Func<T> alternativeFactory)
        {
            MaybeThrowHelper.VerifyAlternativeFactoryNotNull(alternativeFactory);

            if (_task is null)
                return new ValueTask<T>(alternativeFactory());

            return new ValueTask<T>(_AwaitOrElseWithFactory(alternativeFactory));
        }

        /// <summary>
        /// Returns the internal value or constructs an alternative
        /// via the <paramref name="alternativeFactory"/> if this
        /// <see cref="AsyncMaybe{T}"/> is Nothing.
        /// </summary>
        public ValueTask<T> OrElseAsync(
            Func<Task<T>> alternativeFactory)
        {
            MaybeThrowHelper.VerifyAlternativeFactoryNotNull(alternativeFactory);

            if (_task is null)
                return new ValueTask<T>(alternativeFactory());

            return new ValueTask<T>(_AwaitOrElseWithAsyncFactory(alternativeFactory));
        }

        private async Task<T> _AwaitOrElse(T alternative)
        {
            var maybe = await _task.ConfigureAwait(false);
            return maybe.OrElse(alternative);
        }

        private async Task<T> _AwaitOrElseWithFactory(Func<T> alternativeFactory)
        {
            var maybe = await _task.ConfigureAwait(false);
            return maybe.OrElse(alternativeFactory);
        }

        private async Task<T> _AwaitOrElseWithAsyncFactory(Func<Task<T>> alternativeFactory)
        {
            var maybe = await _task.ConfigureAwait(false);
            return await maybe.OrElseAsync(alternativeFactory).ConfigureAwait(false);
        }

        #endregion
    }
}