using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    /// <summary>
    /// An abstraction for accessing persistent storage in the Moralis SDK.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Cleans up any temporary files and/or directories created during SDK operation.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Gets the file wrapper for the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The relative path to the target file</param>
        /// <returns>An instance of <see cref="FileInfo"/> wrapping the the <paramref name="path"/> value</returns>
        FileInfo GetRelativeFile(string path);

        /// <summary>
        /// Transfers a file from <paramref name="originFilePath"/> to <paramref name="targetFilePath"/>.
        /// </summary>
        /// <param name="originFilePath"></param>
        /// <param name="targetFilePath"></param>
        /// <returns>A task that completes once the file move operation form <paramref name="originFilePath"/> to <paramref name="targetFilePath"/> completes.</returns>
        UniTask TransferAsync(string originFilePath, string targetFilePath);

        /// <summary>
        /// Load the contents of this storage controller asynchronously.
        /// </summary>
        /// <returns></returns>
        UniTask<IDataCache<string, object>> LoadAsync();

        /// <summary>
        /// Overwrites the contents of this storage controller asynchronously.
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        UniTask<IDataCache<string, object>> SaveAsync(IDictionary<string, object> contents);
    }

}
