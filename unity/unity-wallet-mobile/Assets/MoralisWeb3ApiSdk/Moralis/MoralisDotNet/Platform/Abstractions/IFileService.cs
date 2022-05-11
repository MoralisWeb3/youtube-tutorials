using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moralis.Platform.Objects;

namespace Moralis.Platform.Abstractions
{
    public interface IFileService
    {
        Task<MoralisFileState> SaveAsync(MoralisFileState state, Stream dataStream, string sessionToken, IProgress<IDataTransferLevel> progress, CancellationToken cancellationToken);
    }
}
