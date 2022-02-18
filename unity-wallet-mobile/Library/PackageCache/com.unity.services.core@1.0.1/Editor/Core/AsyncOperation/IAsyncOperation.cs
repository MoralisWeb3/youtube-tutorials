using System;
using System.Collections;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Contract for an asynchronous operation with no result.
    /// </summary>
    interface IAsyncOperation : IEnumerator
    {
        /// <summary>
        /// If true, this operation either succeeded, failed, or has been canceled.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// The current status of this operation.
        /// </summary>
        AsyncOperationStatus Status { get; }

        /// <summary>
        /// Event raised when the operation succeeded or failed.
        /// The argument is the operation that raised the event.
        /// </summary>
        event Action<IAsyncOperation> Completed;

        /// <summary>
        /// The exception that occured during the operation if it failed.
        /// </summary>
        Exception Exception { get; }
    }

    /// <summary>
    /// Contract for an asynchronous operation returning a result.
    /// </summary>
    /// <typeparam name="T">
    /// The result's type of this operation.
    /// </typeparam>
    interface IAsyncOperation<out T> : IEnumerator
    {
        /// <inheritdoc cref="IAsyncOperation.IsDone"/>
        bool IsDone { get; }

        /// <inheritdoc cref="IAsyncOperation.Status"/>
        AsyncOperationStatus Status { get; }

        /// <summary>
        /// Event raised when the operation succeeded or failed.
        /// The argument is the operation that raised the event.
        /// </summary>
        event Action<IAsyncOperation<T>> Completed;

        /// <inheritdoc cref="IAsyncOperation.Exception"/>
        Exception Exception { get; }

        /// <summary>
        /// The result of this operation.
        /// </summary>
        T Result { get; }
    }
}
