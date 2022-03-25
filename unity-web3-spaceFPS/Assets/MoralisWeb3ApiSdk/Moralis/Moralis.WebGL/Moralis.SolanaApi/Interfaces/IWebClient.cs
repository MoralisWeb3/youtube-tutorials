using System;
using Moralis.WebGL.SolanaApi.Models;
using Status = System.Net.HttpStatusCode;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Moralis.WebGL.SolanaApi.Interfaces
{
    public interface IWebClient
    {
        /// <summary>
        /// Executes HTTP request to a <see cref="WebRequest.Target"/> with <see cref="WebRequest.Method"/> HTTP verb
        /// and <see cref="WebRequest.Headers"/>.
        /// </summary>
        /// <param name="httpRequest">The HTTP request to be executed.</param>
        /// <returns>A task that resolves to Htt</returns>
        UniTask<Tuple<Status, Dictionary<string, string>, string>> ExecuteAsync(WebRequest httpRequest); //, IProgress<IDataTransferLevel> uploadProgress, IProgress<IDataTransferLevel> downloadProgress, CancellationToken cancellationToken = default);
    }
}
