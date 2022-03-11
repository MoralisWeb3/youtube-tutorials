using System;
using System.Threading;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Platform.Utilities;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Services.ClientServices
{
    public static class FileServiceExtensions
    {
        /// <summary>
        /// Saves the file to the Parse cloud.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static async UniTask SaveFileAsync(this IServiceHub<MoralisUser> serviceHub, MoralisFile file, CancellationToken cancellationToken = default)
        {
            await serviceHub.SaveFileAsync(file, default, cancellationToken);
        }

        /// <summary>
        /// Saves the file to the Parse cloud.
        /// </summary>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        //public static Task SaveFileAsync(this IServiceHub<MoralisUser> serviceHub, MoralisFile file, IProgress<IDataTransferLevel> progress, CancellationToken cancellationToken = default) => (file.TaskQueue.Enqueue(toAwait => serviceHub.FileService.SaveAsync(file.State, file.DataStream, serviceHub.GetCurrentSessionTokenAsync(), progress, cancellationToken), cancellationToken).OnSuccess(task => file.State = task.Result));
        public static async UniTask SaveFileAsync(this IServiceHub<MoralisUser> serviceHub, MoralisFile file, IProgress<IDataTransferLevel> progress, CancellationToken cancellationToken = default)
        {
            string sessionToken = await serviceHub.GetCurrentSessionTokenAsync();


            MoralisFileState state = await serviceHub.FileService.SaveAsync(file.State, file.DataStream, sessionToken, progress, cancellationToken);

            file.State = state;
        }
        
        /// <summary>
        /// Saves the file to the Parse cloud.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static async UniTask SaveAsync(this MoralisFile file, IServiceHub<MoralisUser> serviceHub, CancellationToken cancellationToken = default)
        {
            await serviceHub.SaveFileAsync(file, cancellationToken);
        }

        /// <summary>
        /// Saves the file to the Parse cloud.
        /// </summary>
        /// <param name="progress">The progress callback.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public static async UniTask SaveAsync(this MoralisFile file, IServiceHub<MoralisUser> serviceHub, IProgress<IDataTransferLevel> progress, CancellationToken cancellationToken = default)
        {
            await serviceHub.SaveFileAsync(file, progress, cancellationToken);
        }
    }
}
