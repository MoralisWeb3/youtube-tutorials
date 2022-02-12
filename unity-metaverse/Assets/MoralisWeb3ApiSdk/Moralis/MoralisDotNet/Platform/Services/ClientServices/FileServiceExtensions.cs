using System;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Objects;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.ClientServices
{
    public static class FileServiceExtensions
    {
        /// <summary>
        /// Saves the file to the Parse cloud.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static Task SaveFileAsync(this IServiceHub<MoralisUser> serviceHub, MoralisFile file, CancellationToken cancellationToken = default) => serviceHub.SaveFileAsync(file, default, cancellationToken);

        /// <summary>
        /// Saves the file to the Parse cloud.
        /// </summary>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        //public static Task SaveFileAsync(this IServiceHub<MoralisUser> serviceHub, MoralisFile file, IProgress<IDataTransferLevel> progress, CancellationToken cancellationToken = default) => (file.TaskQueue.Enqueue(toAwait => serviceHub.FileService.SaveAsync(file.State, file.DataStream, serviceHub.GetCurrentSessionTokenAsync(), progress, cancellationToken), cancellationToken).OnSuccess(task => file.State = task.Result));
        public static Task SaveFileAsync(this IServiceHub<MoralisUser> serviceHub, MoralisFile file, IProgress<IDataTransferLevel> progress, CancellationToken cancellationToken = default) => serviceHub.GetCurrentSessionTokenAsync().OnSuccess(sessionTask => file.TaskQueue.Enqueue(toAwait => serviceHub.FileService.SaveAsync(file.State, file.DataStream, sessionTask.Result, progress, cancellationToken), cancellationToken).OnSuccess(task => file.State = task.Result));
        //public static Task SaveFileAsync(this IServiceHub serviceHub, ParseFile file, IProgress<IDataTransferLevel> progress, CancellationToken cancellationToken = default) => file.TaskQueue.Enqueue(toAwait => serviceHub.FileController.SaveAsync(file.State, file.DataStream, serviceHub.GetCurrentSessionToken(), progress, cancellationToken), cancellationToken).OnSuccess(task => file.State = task.Result);

#warning Make serviceHub null by default once dependents properly inject it when needed.

        /// <summary>
        /// Saves the file to the Parse cloud.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static Task SaveAsync(this MoralisFile file, IServiceHub<MoralisUser> serviceHub, CancellationToken cancellationToken = default) => serviceHub.SaveFileAsync(file, cancellationToken);

        /// <summary>
        /// Saves the file to the Parse cloud.
        /// </summary>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static Task SaveAsync(this MoralisFile file, IServiceHub<MoralisUser> serviceHub, IProgress<IDataTransferLevel> progress, CancellationToken cancellationToken = default) => serviceHub.SaveFileAsync(file, progress, cancellationToken);
    }
}
