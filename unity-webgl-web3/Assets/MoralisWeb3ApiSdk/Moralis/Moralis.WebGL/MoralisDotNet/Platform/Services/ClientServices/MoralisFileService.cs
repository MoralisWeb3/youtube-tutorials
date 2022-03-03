using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Moralis.WebGL.Platform.Abstractions;
using Moralis.WebGL.Platform.Exceptions;
using Moralis.WebGL.Platform.Objects;
using Moralis.WebGL.Platform.Services.ClientServices;
using Moralis.WebGL.Platform.Services.Models;
using Moralis.WebGL.Platform.Utilities;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Services.ClientServices
{
    public class MoralisFileService : IFileService
    {
        IMoralisCommandRunner CommandRunner { get; }

        IJsonSerializer JsonSerializer { get; }

        public MoralisFileService(IMoralisCommandRunner commandRunner, IJsonSerializer jsonSerializer) => (CommandRunner, JsonSerializer) = (commandRunner, jsonSerializer);

        public async UniTask<MoralisFileState> SaveAsync(MoralisFileState state, Stream dataStream, string sessionToken, IProgress<IDataTransferLevel> progress, CancellationToken cancellationToken = default)
        {
            if (state.url != null)
                return state;

            if (cancellationToken.IsCancellationRequested)
                return await UniTask.FromCanceled<MoralisFileState>(cancellationToken);

            long oldPosition = dataStream.Position;

            Tuple<HttpStatusCode, string> cmdResult = await CommandRunner.RunCommandAsync(new MoralisCommand($"server/files/{state.name}", method: "POST", sessionToken: sessionToken, contentType: state.mediatype, stream: dataStream), uploadProgress: progress, cancellationToken: cancellationToken);
                
            cancellationToken.ThrowIfCancellationRequested();
            MoralisFileState fileState = default;

            if (cmdResult.Item2 is { })
            {
                fileState = JsonSerializer.Deserialize<MoralisFileState>(cmdResult.Item2);

                if (String.IsNullOrWhiteSpace(fileState.name) || !(fileState.url is { }))
                    throw new MoralisFailureException(MoralisFailureException.ErrorCode.ScriptFailed, "");

                fileState.mediatype = state.mediatype;
            }
            else
                throw new MoralisFailureException(MoralisFailureException.ErrorCode.ScriptFailed, "");
        
            // Rewind the stream on failure or cancellation (if possible).
            if (dataStream.CanSeek)
                dataStream.Seek(oldPosition, SeekOrigin.Begin);

            return fileState;
        }
    }
}
