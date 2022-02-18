using System;
using System.Threading.Tasks;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Set of utility functions added as extensions to
    /// <see cref="IAsyncOperation"/> and <see cref="IAsyncOperation{T}"/>.
    /// </summary>
    static class AsyncOperationExtensions
    {
        /// <summary>
        /// Get a default awaiter on <paramref name="self"/>.
        /// </summary>
        /// <param name="self">
        /// The operation to create an awaiter for.
        /// </param>
        /// <returns>
        /// Return a default awaiter for <paramref name="self"/>.
        /// </returns>
        /// <remarks>
        /// This is required so we can directly use the <see langword="await"/>
        /// keyword on an <see cref="IAsyncOperation"/>.
        /// </remarks>
        public static AsyncOperationAwaiter GetAwaiter(this IAsyncOperation self)
        {
            return new AsyncOperationAwaiter(self);
        }

        /// <summary>
        /// Get a Task based on <paramref name="self"/>.
        /// </summary>
        /// <param name="self">
        /// The operation to create a Task for.
        /// </param>
        /// <returns>
        /// Return a <see cref="T:System.Threading.Tasks.Task"/>.
        /// </returns>
        public static Task AsTask(this IAsyncOperation self)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();

            void CompleteTask(IAsyncOperation operation)
            {
                switch (operation.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        taskCompletionSource.TrySetResult(null);
                        break;
                    case AsyncOperationStatus.Failed:
                        taskCompletionSource.TrySetException(operation.Exception);
                        break;
                    case AsyncOperationStatus.Cancelled:
                        taskCompletionSource.TrySetCanceled();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (self.IsDone)
            {
                CompleteTask(self);
            }
            else
            {
                self.Completed += CompleteTask;
            }

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Get a default awaiter for <paramref name="self"/>.
        /// </summary>
        /// <param name="self">
        /// The operation to create an awaiter for.
        /// </param>
        /// <typeparam name="T">
        /// The result's type of <paramref name="self"/>.
        /// </typeparam>
        /// <returns>
        /// Return a default awaiter for <paramref name="self"/>.
        /// </returns>
        /// <remarks>
        /// This is required so we can directly use the <see langword="await"/>
        /// keyword on an <see cref="IAsyncOperation{T}"/>.
        /// </remarks>
        public static AsyncOperationAwaiter<T> GetAwaiter<T>(this IAsyncOperation<T> self)
        {
            return new AsyncOperationAwaiter<T>(self);
        }

        /// <summary>
        /// Get a Task based on <paramref name="self"/>.
        /// </summary>
        /// <param name="self">
        /// The operation to create a Task for.
        /// </param>
        /// <typeparam name="T">
        /// The result's type of <paramref name="self"/>.
        /// </typeparam>
        /// <returns>
        /// Return a <see cref="T:System.Threading.Tasks.Task`1"/>
        /// </returns>
        public static Task<T> AsTask<T>(this IAsyncOperation<T> self)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();

            void CompleteTask(IAsyncOperation<T> operation)
            {
                switch (operation.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        taskCompletionSource.TrySetResult(operation.Result);
                        break;
                    case AsyncOperationStatus.Failed:
                        taskCompletionSource.TrySetException(operation.Exception);
                        break;
                    case AsyncOperationStatus.Cancelled:
                        taskCompletionSource.TrySetCanceled();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (self.IsDone)
            {
                CompleteTask(self);
            }
            else
            {
                self.Completed += CompleteTask;
            }

            return taskCompletionSource.Task;
        }
    }
}
