using System.Runtime.CompilerServices;

namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// Contract for objects allowing to use the <see langword="await"/> keyword on an <see cref="IAsyncOperation"/>.
    /// </summary>
    /// <remarks>
    /// For more information, see <see href="https://github.com/dotnet/roslyn/blob/master/docs/features/task-types.md"/>
    /// </remarks>
    interface IAsyncOperationAwaiter : ICriticalNotifyCompletion
    {
        /// <inheritdoc cref="IAsyncOperation.IsDone"/>
        bool IsCompleted { get; }

        /// <summary>
        /// Get the operation's current result.
        /// </summary>
        /// <remarks>
        /// * Does nothing on success but must be declared to match the awaiter pattern.
        /// * Is expected to throw if the operation failed or has been canceled.
        /// </remarks>
        void GetResult();
    }

    /// <summary>
    /// Contract for objects allowing to use the <see langword="await"/> keyword on an <see cref="IAsyncOperation{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The result's type of the awaited operation.
    /// </typeparam>
    /// <remarks>
    /// For more information, see <see href="https://github.com/dotnet/roslyn/blob/master/docs/features/task-types.md"/>
    /// </remarks>
    interface IAsyncOperationAwaiter<out T> : ICriticalNotifyCompletion
    {
        /// <inheritdoc cref="IAsyncOperation.IsDone"/>
        bool IsCompleted { get; }

        /// <summary>
        /// Get the operation's current result.
        /// </summary>
        /// <returns>
        /// Return the operation's current <see cref="IAsyncOperation{T}.Result"/>.
        /// </returns>
        /// <remarks>
        /// Is expected to throw if the operation failed or has been canceled.
        /// </remarks>
        T GetResult();
    }
}
