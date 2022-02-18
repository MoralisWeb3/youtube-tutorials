using System;
using System.Collections;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Default implementation for <see cref="IAsyncOperation"/>.
    /// </summary>
    class AsyncOperation : IAsyncOperation
    {
        /// <inheritdoc/>
        public bool IsDone { get; protected set; }

        /// <inheritdoc/>
        public AsyncOperationStatus Status { get; protected set; }

        /// <inheritdoc/>
        public event Action<IAsyncOperation> Completed
        {
            add
            {
                if (IsDone)
                    value(this);
                else
                    m_CompletedCallback += value;
            }
            remove => m_CompletedCallback -= value;
        }

        /// <inheritdoc/>
        public Exception Exception { get; protected set; }

        /// <inheritdoc cref="IAsyncOperation.Completed"/>
        protected Action<IAsyncOperation> m_CompletedCallback;

        /// <summary>
        /// Set this operation's status <see cref="AsyncOperationStatus.InProgress"/>.
        /// </summary>
        public void SetInProgress()
        {
            Status = AsyncOperationStatus.InProgress;
        }

        /// <summary>
        /// Complete this operation as a success.
        /// </summary>
        public void Succeed()
        {
            if (IsDone)
            {
                return;
            }

            IsDone = true;
            Status = AsyncOperationStatus.Succeeded;
            m_CompletedCallback?.Invoke(this);
            m_CompletedCallback = null;
        }

        /// <summary>
        /// Complete this operation as a failure using the given <paramref name="reason"/>.
        /// </summary>
        /// <param name="reason">
        /// The exception at the source of the failure.
        /// </param>
        public void Fail(Exception reason)
        {
            if (IsDone)
            {
                return;
            }

            Exception = reason;
            IsDone = true;
            Status = AsyncOperationStatus.Failed;
            m_CompletedCallback?.Invoke(this);
            m_CompletedCallback = null;
        }

        /// <summary>
        /// Complete this operation as a cancellation.
        /// </summary>
        public void Cancel()
        {
            if (IsDone)
            {
                return;
            }

            Exception = new OperationCanceledException();
            IsDone = true;
            Status = AsyncOperationStatus.Cancelled;
            m_CompletedCallback?.Invoke(this);
            m_CompletedCallback = null;
        }

        /// <inheritdoc/>
        bool IEnumerator.MoveNext() => !IsDone;

        /// <inheritdoc/>
        /// <remarks>
        /// Left empty because we don't support operation reset.
        /// </remarks>
        void IEnumerator.Reset() {}

        /// <inheritdoc/>
        object IEnumerator.Current => null;
    }

    /// <summary>
    /// Default implementation for <see cref="IAsyncOperation{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The result's type of this operation.
    /// </typeparam>
    class AsyncOperation<T> : IAsyncOperation<T>
    {
        /// <inheritdoc/>
        public bool IsDone { get; protected set; }

        /// <inheritdoc/>
        public AsyncOperationStatus Status { get; protected set; }

        /// <inheritdoc/>
        public event Action<IAsyncOperation<T>> Completed
        {
            add
            {
                if (IsDone)
                    value(this);
                else
                    m_CompletedCallback += value;
            }
            remove => m_CompletedCallback -= value;
        }

        /// <inheritdoc/>
        public Exception Exception { get; protected set; }

        /// <inheritdoc/>
        public T Result { get; protected set; }

        /// <inheritdoc cref="IAsyncOperation{T}.Completed"/>
        protected Action<IAsyncOperation<T>> m_CompletedCallback;

        /// <summary>
        /// Set this operation's status <see cref="AsyncOperationStatus.InProgress"/>.
        /// </summary>
        public void SetInProgress()
        {
            Status = AsyncOperationStatus.InProgress;
        }

        /// <summary>
        /// Complete this operation as a success and set its result.
        /// </summary>
        /// <param name="result">
        /// The result of this operation.
        /// </param>
        public void Succeed(T result)
        {
            if (IsDone)
            {
                return;
            }

            Result = result;
            IsDone = true;
            Status = AsyncOperationStatus.Succeeded;
            m_CompletedCallback?.Invoke(this);
            m_CompletedCallback = null;
        }

        /// <summary>
        /// Complete this operation as a failure using the given <paramref name="reason"/>.
        /// </summary>
        /// <param name="reason">
        /// The exception at the source of the failure.
        /// </param>
        public void Fail(Exception reason)
        {
            if (IsDone)
            {
                return;
            }

            Exception = reason;
            IsDone = true;
            Status = AsyncOperationStatus.Failed;
            m_CompletedCallback?.Invoke(this);
            m_CompletedCallback = null;
        }

        /// <summary>
        /// Complete this operation as a cancellation.
        /// </summary>
        public void Cancel()
        {
            if (IsDone)
            {
                return;
            }

            Exception = new OperationCanceledException();
            IsDone = true;
            Status = AsyncOperationStatus.Cancelled;
            m_CompletedCallback?.Invoke(this);
            m_CompletedCallback = null;
        }

        /// <inheritdoc/>
        bool IEnumerator.MoveNext() => !IsDone;

        /// <inheritdoc/>
        /// <remarks>
        /// Left empty because we don't support operation reset.
        /// </remarks>
        void IEnumerator.Reset() {}

        /// <inheritdoc/>
        object IEnumerator.Current => null;
    }
}
