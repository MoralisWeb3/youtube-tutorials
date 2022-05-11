using System;
using System.Threading;
using Moralis.WebGL.Platform.Services.Models;
using Status = System.Net.HttpStatusCode;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface IWebClient
    {
        /// <summary>
        /// Executes HTTP request to a <see cref="WebRequest.Target"/> with <see cref="WebRequest.Method"/> HTTP verb
        /// and <see cref="WebRequest.Headers"/>.
        /// </summary>
        /// <param name="httpRequest">The HTTP request to be executed.</param>
        /// <returns>A task that resolves to Htt</returns>
        UniTask<Tuple<Status, string>> ExecuteAsync(WebRequest httpRequest); //, IProgress<IDataTransferLevel> uploadProgress, IProgress<IDataTransferLevel> downloadProgress, CancellationToken cancellationToken = default);
    }
}
