using System;
using System.IO;
using System.Threading;
using Moralis.WebGL.Platform.Objects;
using Cysharp.Threading.Tasks;

namespace Moralis.WebGL.Platform.Abstractions
{
    public interface IFileService
    {
        UniTask<MoralisFileState> SaveAsync(MoralisFileState state, Stream dataStream, string sessionToken, IProgress<IDataTransferLevel> progress, CancellationToken cancellationToken);
    }
}
