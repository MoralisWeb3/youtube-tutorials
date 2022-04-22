using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Abstractions;
using Moralis.Platform.Exceptions;
using Moralis.Platform.Objects;
using Moralis.Platform.Services.ClientServices;
using Moralis.Platform.Services.Models;
using Moralis.Platform.Utilities;

namespace Moralis.Platform.Services.ClientServices
{
    public class MoralisFileService : IFileService
    {
        IMoralisCommandRunner CommandRunner { get; }

        IJsonSerializer JsonSerializer { get; }

        public MoralisFileService(IMoralisCommandRunner commandRunner, IJsonSerializer jsonSerializer) => (CommandRunner, JsonSerializer) = (commandRunner, jsonSerializer);

        public Task<MoralisFileState> SaveAsync(MoralisFileState state, Stream dataStream, string sessionToken, IProgress<IDataTransferLevel> progress, CancellationToken cancellationToken = default)
        {
            if (state.url != null)
                // !isDirty

                return Task.FromResult(state);

            if (cancellationToken.IsCancellationRequested)
                return Task.FromCanceled<MoralisFileState>(cancellationToken);

            long oldPosition = dataStream.Position;

            return CommandRunner.RunCommandAsync(new MoralisCommand($"server/files/{state.name}", method: "POST", sessionToken: sessionToken, contentType: state.mediatype, stream: dataStream), uploadProgress: progress, cancellationToken: cancellationToken).OnSuccess(uploadTask =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                Tuple<HttpStatusCode, string> result = uploadTask.Result;
                if (result.Item2 is { })
                {
                    MoralisFileState resp = JsonSerializer.Deserialize<MoralisFileState>(result.Item2);

                    if (String.IsNullOrWhiteSpace(resp.name) || !(resp.url is { }))
                        throw new MoralisFailureException(MoralisFailureException.ErrorCode.ScriptFailed, "");

                    resp.mediatype = state.mediatype;
                    return resp;
                }
                else
                    throw new MoralisFailureException(MoralisFailureException.ErrorCode.ScriptFailed, "");
            }).ContinueWith(task =>
            {
                // Rewind the stream on failure or cancellation (if possible).

                if ((task.IsFaulted || task.IsCanceled) && dataStream.CanSeek)
                    dataStream.Seek(oldPosition, SeekOrigin.Begin);

                return task;
            }).Unwrap();
        }
    }
}
