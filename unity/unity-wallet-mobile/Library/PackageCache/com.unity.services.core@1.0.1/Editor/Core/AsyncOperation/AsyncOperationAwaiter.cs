using System;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Default implementation for <see cref="IAsyncOperationAwaiter"/>.
    /// </summary>
    struct AsyncOperationAwaiter : IAsyncOperationAwaiter
    {
        IAsyncOperation m_Operation;

        /// <summary>
        /// Create an awaiter for the given <paramref name="asyncOperation"/>.
        /// </summary>
        /// <param name="asyncOperation">
        /// The operation to await.
        /// </param>
        public AsyncOperationAwaiter(IAsyncOperation asyncOperation)
        {
            m_Operation = asyncOperation;
        }

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">
        /// The action to invoke when the operation completes.
        /// </param>
        /// <remarks>
        /// Straightforward implementation of <see cref="System.Runtime.CompilerServices.ICriticalNotifyCompletion"/>.
        /// </remarks>
        public void OnCompleted(Action continuation)
        {
            m_Operation.Completed += operation => continuation();
        }

        /// <inheritdoc cref="OnCompleted"/>
        public void UnsafeOnCompleted(Action continuation)
        {
            m_Operation.Completed += operation => continuation();
        }

        /// <inheritdoc/>
        public bool IsCompleted => m_Operation.IsDone;

        /// <inheritdoc/>
        public void GetResult()
        {
            if (m_Operation.Status == AsyncOperationStatus.Failed
                || m_Operation.Status == AsyncOperationStatus.Cancelled)
            {
                throw m_Operation.Exception;
            }
        }
    }

    /// <summary>
    /// Default implementation for <see cref="IAsyncOperationAwaiter{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The result's type of the awaited operation.
    /// </typeparam>
    struct AsyncOperationAwaiter<T> : IAsyncOperationAwaiter<T>
    {
        IAsyncOperation<T> m_Operation;

        /// <summary>
        /// Create an awaiter for the given <paramref name="asyncOperation"/>.
        /// </summary>
        /// <param name="asyncOperation">
        /// The operation to await.
        /// </param>
        public AsyncOperationAwaiter(IAsyncOperation<T> asyncOperation)
        {
            m_Operation = asyncOperation;
        }

        /// <inheritdoc cref="AsyncOperationAwaiter.OnCompleted"/>
        public void OnCompleted(Action continuation)
        {
            m_Operation.Completed += obj => continuation();
        }

        /// <inheritdoc cref="AsyncOperationAwaiter.UnsafeOnCompleted"/>
        public void UnsafeOnCompleted(Action continuation)
        {
            m_Operation.Completed += obj => continuation();
        }

        /// <inheritdoc/>
        public bool IsCompleted => m_Operation.IsDone;

        /// <inheritdoc/>
        public T GetResult()
        {
            if (m_Operation.Status == AsyncOperationStatus.Failed
                || m_Operation.Status == AsyncOperationStatus.Cancelled)
            {
                throw m_Operation.Exception;
            }

            return m_Operation.Result;
        }
    }
}
