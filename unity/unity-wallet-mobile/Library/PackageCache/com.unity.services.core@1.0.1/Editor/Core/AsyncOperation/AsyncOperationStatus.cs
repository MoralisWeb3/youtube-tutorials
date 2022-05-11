namespace Unity.Services.Core.Editor
{
    /// <summary>
    /// All supported status of an <see cref="IAsyncOperation"/>.
    /// </summary>
    enum AsyncOperationStatus
    {
        /// <summary>
        /// The operation status hasn't been defined yet.
        /// </summary>
        None,
        /// <summary>
        /// The operation is running.
        /// </summary>
        InProgress,
        /// <summary>
        /// The operation is completed without any errors.
        /// </summary>
        Succeeded,
        /// <summary>
        /// The operation is completed with errors.
        /// </summary>
        Failed,
        /// <summary>
        /// The operation has been canceled.
        /// </summary>
        Cancelled
    }
}
